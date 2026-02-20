using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Common.Validators;
using SmartGrid.Application.Interfaces;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;
using SmartGrid.Domain.Models;

namespace SmartGrid.Application.Features.Devices.Commands
{
    // COMMAND
    public record AddNewDeviceCommand : IRequest<Result<string>>
    {
        public string Name { get; init; } = string.Empty;
        public DeviceType DeviceType { get; init; }
        public double NominalPower { get; init; }
        public string Location { get; init; } = string.Empty;
    }

    // VALIDATOR
    public class AddNewDeviceValidator : AbstractValidator<AddNewDeviceCommand>
    {
        public AddNewDeviceValidator()
        {
            RuleFor(t => t.Name)
                .NotEmpty().WithMessage("Device name is required.");

            RuleFor(t => t.Location)
                .NotEmpty().WithMessage("Location is required.");

            RuleFor(t => t.DeviceType)
                .IsValidDeviceType();

            RuleFor(t => t.NominalPower)
                .NotEmpty().WithMessage("NominalPower is missing from payload.")
                .GreaterThan(0).WithMessage("NominalPower must be greater than zero.");
        }
    }

    // HANDLER
    internal class AddNewDeviceHandler(
        IDeviceRepository deviceRepository,
        IFirmwareRepository firmwareRepository,
        IDateTimeProvider dateTimeProvider,
        ILogger<AddNewDeviceHandler> logger) : IRequestHandler<AddNewDeviceCommand, Result<string>>
    {

        public async Task<Result<string>> Handle(AddNewDeviceCommand request, CancellationToken ct)
        {
            var now = dateTimeProvider.UtcNow;

            var deviceResult = Device.Create(
                request.DeviceType,
                request.Name,
                request.NominalPower,
                request.Location,
                now
            );

            if (deviceResult.IsFailure)
                return Result<string>.Failure(deviceResult.Error!.Message, ErrorType.Validation);

            var device = deviceResult.Value;

            try
            {
                var latestVersion = await firmwareRepository.GetLatestVersionAsync(device.Type, ct);

                if (latestVersion is not null)
                {
                    device.ApplyInitialFirmware(latestVersion);
                }

                await deviceRepository.SaveAsync(device, ct);

                logger.LogInformation("[DEVICE] New device {Name} (ID: {Id}) successfully registered.",
                    device.Name, device.Id);

                return Result<string>.Success(device.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while processing new device.");
                return Result<string>.Failure("Failed to process new device.", ErrorType.Failure);
            }
        }
    }
}
