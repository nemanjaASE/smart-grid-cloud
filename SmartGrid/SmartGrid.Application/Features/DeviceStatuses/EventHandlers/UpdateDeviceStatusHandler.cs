using MediatR;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Features.Telemetries.Events;
using SmartGrid.Application.Interfaces.Repositories;

namespace SmartGrid.Application.Features.DeviceStatuses.EventHandlers;

internal class UpdateDeviceStatusHandler(
        IDeviceRepository deviceRepository,
        ILogger<UpdateDeviceStatusHandler> logger) : INotificationHandler<TelemetryProcessedEvent>
{
    public async Task Handle(TelemetryProcessedEvent notification, CancellationToken ct)
    {
        var telemetry = notification.Telemetry;

        try
        {
            var device = await deviceRepository.GetWithStatusByIdAsync(
                telemetry.DeviceType,
                telemetry.DeviceId,
                ct);

            if (device is null)
            {
                logger.LogWarning("Cannot update status: Device {DeviceId} of type {DeviceType} not found.",
                    telemetry.DeviceId, telemetry.DeviceType);
                return;
            }

            device.ProcessTelemetry(telemetry);

            await deviceRepository.SaveStatusAsync(device.Status, ct);

            logger.LogInformation("Successfully updated status for device {DeviceId}.", telemetry.DeviceId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error asynchronously updating status for device {DeviceId}", telemetry.DeviceId);
        }
    }
}