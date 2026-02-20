using MediatR;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Interfaces;
using SmartGrid.Application.Interfaces.Messaging;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;

namespace SmartGrid.Application.Features.DeviceStatuses.Commands;

// COMMAND - Empty
public record ExecuteMonitoringCommand : IRequest<Result>;

// HANDLER
internal class ExecuteMonitoringHandler(
    IDeviceRepository deviceRepository,
    IDomainEventDispatcher dispatcher,
    IDeviceStatusQueueService deviceStatusQueueService,
    IParallelSettingsProvider parallelSettings,
    ILogger<ExecuteMonitoringHandler> logger,
    IDateTimeProvider dateTimeProvider
    ) : IRequestHandler<ExecuteMonitoringCommand, Result>
{
    public async Task<Result> Handle(ExecuteMonitoringCommand request, CancellationToken ct)
    {
        try
        {
            var devices = deviceRepository.GetAllWithStatusStreamingAsync(ct);

            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = parallelSettings.MaxDegreeOfParallelism,
                CancellationToken = ct
            };

            await Parallel.ForEachAsync(devices, options, async (device, token) =>
            {
                try
                {
                    await deviceStatusQueueService.SendStatusUpdateAsync(device.Status, token);

                    device.EvaluateMonitoring(dateTimeProvider.UtcNow);

                    if (device.DomainEvents.Count != 0)
                    {
                        await dispatcher.DispatchAsync(device.DomainEvents, token);
                        device.ClearDomainEvents();
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "[MONITORING] Failed processing for Device {Id}", device.Id.Value);
                }
            });

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Monitoring cycle failed due to an unexpected error.");
            return Result.Failure("Critical monitoring failure.", ErrorType.Failure);
        }
    }
}