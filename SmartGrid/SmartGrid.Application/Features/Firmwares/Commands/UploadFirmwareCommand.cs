using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Common;
using SmartGrid.Application.Common.Validators;
using SmartGrid.Application.Interfaces;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Application.Interfaces.Storage;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;
using SmartGrid.Domain.Models;

namespace SmartGrid.Application.Features.Firmwares.Commands
{
    // COMMAND
    public record UploadFirmwareCommand : IRequest<Result>
    {
        public UploadedFirmwareFile FirmwareFile { get; init; } = new();
        public DeviceType DeviceType { get; init; } = DeviceType.Unknown;
        public string Version { get; init; } = string.Empty;
    }

    // VALIDATOR
    public class UploadFirmwareValidator : AbstractValidator<UploadFirmwareCommand>
    {
        public UploadFirmwareValidator()
        {
            RuleFor(t => t.DeviceType)
                .IsValidDeviceType();

            RuleFor(t => t.Version)
                .IsValidFirmwareVersion();

            RuleFor(t => t.FirmwareFile)
                .NotNull().WithMessage("Firmware file is required.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.FirmwareFile.Content.Length)
                        .GreaterThan(0).WithMessage("File cannot be empty.")
                        .Must(x => x <= 10 * 1024 * 1024).WithMessage("File size must not exceed 10 MB.");

                    RuleFor(x => x.FirmwareFile.FileName)
                        .Must(name => name.EndsWith(".bin", StringComparison.OrdinalIgnoreCase))
                        .WithMessage("Only .bin files are allowed.");
                });
        }
    }

    // HANDLER
    internal class UploadFirmwareHandler(
        IFirmwareBlobStorage firmwareStorage,
        IFirmwareRepository firmwareRepository,
        IDateTimeProvider dateTimeProvider,
        ILogger<UploadFirmwareHandler> logger) : IRequestHandler<UploadFirmwareCommand, Result>
    {
        public async Task<Result> Handle(UploadFirmwareCommand request, CancellationToken ct)
        {
            var metadata = new FirmwareMetadata()
            {
                DeviceType = request.DeviceType.ToString(),
                Version = request.Version
            };

            bool exists = await firmwareStorage.ExistsAsync(metadata, ct);

            if (exists)
            {
                logger.LogWarning("[FIRMWARE] Upload aborted. File already exists for {DeviceType} v{Version}",
                    request.DeviceType, request.Version);
                return Result.Failure(
                    $"Firmware version {request.Version} for {request.DeviceType} already exists.",
                    ErrorType.Validation);
            }

            var fileData = new FileData<FirmwareMetadata>()
            {
                Content = request.FirmwareFile.Content,
                Metadata = metadata,
            };

            try
            {
                await firmwareStorage.SaveAsync(fileData, ct);

                var firmwareResult = Firmware.Create(
                    request.DeviceType,
                    request.Version,
                    request.FirmwareFile.FileName,
                    request.FirmwareFile.Content.Length,
                    dateTimeProvider.UtcNow
                );

                if (firmwareResult.IsFailure)
                    return Result.Failure(firmwareResult.Error!.Message, ErrorType.Validation);

                var firmware = firmwareResult.Value;

                await firmwareRepository.SaveAsync(firmware, ct);

                return Result.Success();
            }
            catch (Exception ex)
            {
                await firmwareStorage.DeleteAsync(metadata, ct);
                logger.LogError(ex, "Error while processing new upload.");
                    return Result.Failure("Failed to process new uploaded version of firmware", ErrorType.Failure);
            }
        }
    }
}
