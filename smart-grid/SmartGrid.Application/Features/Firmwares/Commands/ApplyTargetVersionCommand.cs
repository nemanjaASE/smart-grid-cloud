using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Common.Validators;
using SmartGrid.Application.Interfaces;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;
using SmartGrid.Domain.ValueObjects;
using System.Collections.Concurrent;

namespace SmartGrid.Application.Features.Firmwares.Commands
{
    // COMMAND
    public record ApplyTargetVersionCommand(DeviceType DeviceType, string NewVersion) : IRequest<Result<BulkUpdateResult>>;

    // VALIDATOR
    public class ApplyTargetVersionValidator : AbstractValidator<ApplyTargetVersionCommand>
    {
        public ApplyTargetVersionValidator()
        {
            RuleFor(t => t.DeviceType)
                .IsValidDeviceType();

            RuleFor(t => t.NewVersion)
                .IsValidFirmwareVersion();
        }
    }

    // HANDLER
    internal class ApplyTargetVersionHandler(
        IDeviceStatusQueryRepository deviceStatusQueryRepository,
        IDeviceRepository deviceRepository,
        IFirmwareRepository firmwareRepository,
        IParallelSettingsProvider parallelSettings,
        ILogger<ApplyTargetVersionCommand> logger) : IRequestHandler<ApplyTargetVersionCommand, Result<BulkUpdateResult>>
    {
        public async Task<Result<BulkUpdateResult>> Handle(ApplyTargetVersionCommand request, CancellationToken ct)
        {
            var requestedVersionRes = FirmwareVersion.Create(request.NewVersion);
            if (requestedVersionRes.IsFailure)
                return Result<BulkUpdateResult>.Failure(requestedVersionRes.Error!.Message, ErrorType.Validation);

            var latestInDb = await firmwareRepository.GetLatestVersionAsync(request.DeviceType, ct);

            if (latestInDb == null || latestInDb.CompareTo(requestedVersionRes.Value) != 0)
            {
                return Result<BulkUpdateResult>.Failure(
                    $"Version {request.NewVersion} must be uploaded to the firmware repository first.",
                    ErrorType.Validation);
            }

            var statuses = deviceStatusQueryRepository.GetByTypeStreamingAsync(request.DeviceType, ct);
            var version = requestedVersionRes.Value;

            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = parallelSettings.MaxDegreeOfParallelism,
                CancellationToken = ct
            };

            var failedDevices = new ConcurrentBag<string>();
            int total = 0;


            await Parallel.ForEachAsync(statuses, parallelOptions, 
                async (status, token) =>
                {
                    try
                    {
                        Interlocked.Increment(ref total);

                        status.SetTargetVersion(version);
                        await deviceRepository.SaveStatusAsync(status, token);

                        logger.LogInformation("[FIRMWARE] Target version {Ver} set for Device {Id}",
                            version.Value, status.DeviceId.Value);
                    }
                    catch (Exception ex)
                    {
                        failedDevices.Add(status.DeviceId.Value);

                        logger.LogError(ex, "[ERROR] Failed to set target version for Device {Id}", status.DeviceId.Value);
                    }
                }
            );

            var failed = failedDevices.Count;
            var succeeded = total - failed;

            var result = new BulkUpdateResult(total, succeeded, failed);

            if (failed > 0)
            {
                return Result<BulkUpdateResult>.Failure($"Completed with {failed} failures.", ErrorType.Conflict);
            }

            return Result<BulkUpdateResult>.Success(result);
        }
    }
}
