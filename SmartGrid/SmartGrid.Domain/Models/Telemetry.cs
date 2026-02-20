using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;
using SmartGrid.Domain.ValueObjects;

namespace SmartGrid.Domain.Models
{
    public class Telemetry
    {
        public EntityId Id { get; private set; }
        public EntityId DeviceId { get; private set; }
        public string DeviceName { get; private set; } = string.Empty;
        public DeviceType DeviceType { get; private set; } = DeviceType.Unknown;
        public FirmwareVersion FirmwareVersion { get; private set; }
        public Power NominalPower { get; private set; }
        public Power CurrentPower { get; private set; }
        public DateTime Timestamp { get; private set; }
        public Percentage LoadPercentage { get; private set; }

        private Telemetry(
            EntityId id,
            EntityId deviceId,
            string deviceName,
            DeviceType deviceType,
            Power nominalPower,
            Power currentPower,
            DateTime timestamp,
            FirmwareVersion firmwareVersion)
        {
            Id = id;
            DeviceId = deviceId;
            DeviceName = deviceName;
            DeviceType = deviceType;
            NominalPower = nominalPower;
            CurrentPower = currentPower;
            Timestamp = timestamp;
            FirmwareVersion = firmwareVersion;
            LoadPercentage = nominalPower.Value > 0
                ? Percentage.FromRaw(Math.Round((currentPower.Value / nominalPower.Value) * 100, 2))
                : Percentage.Zero();
        }

        #region Factory Method

        public static Result<Telemetry> Create(
            string deviceId,
            string deviceName,
            DeviceType deviceType,
            double nominalPower,
            double currentPower,
            DateTime timestamp,
            string firmwareVersion)
        {
            if (string.IsNullOrWhiteSpace(deviceName))
                return Result<Telemetry>.Failure("DeviceName is required.",
                    ErrorType.Validation);

            if (!Enum.IsDefined(typeof(DeviceType), deviceType)
                || deviceType == DeviceType.Unknown)
                return Result<Telemetry>.Failure("A valid and defined DeviceType must be specified.",
                    ErrorType.Validation);

            if (timestamp > DateTime.UtcNow)
                return Result<Telemetry>.Failure("Timestamp cannot be in the future.",
                    ErrorType.Validation);

            var idResult = EntityId.Create(deviceId);

            if (idResult.IsFailure)
                return Result<Telemetry>.Failure(idResult.Error!.Message, ErrorType.Validation);

            var nominalPowerResult = Power.Create(nominalPower);

            if (nominalPowerResult.IsFailure)
                return Result<Telemetry>.Failure(nominalPowerResult.Error!.Message, ErrorType.Validation);

            var currentPowerResult = Power.Create(currentPower);

            if (currentPowerResult.IsFailure)
                return Result<Telemetry>.Failure(currentPowerResult.Error!.Message, ErrorType.Validation);

            var firmwareVersionResult = FirmwareVersion.Create(firmwareVersion);

            if (firmwareVersionResult.IsFailure)
                return Result<Telemetry>.Failure(firmwareVersionResult.Error!.Message, ErrorType.Validation);

            return Result<Telemetry>.Success(new Telemetry(
                EntityId.New(),
                idResult.Value,
                deviceName,
                deviceType,
                nominalPowerResult.Value,
                currentPowerResult.Value,
                timestamp,
                firmwareVersionResult.Value
            ));
        }
        public static Result<Telemetry> Load(
            string id,
            string deviceId,
            string deviceName,
            DeviceType deviceType,
            double nominalPower,
            double currentPower,
            DateTime timestamp,
            string firmwareVersion)
        {
            var idResult = EntityId.Create(id);
            if (idResult.IsFailure)
                return Result<Telemetry>.Failure(idResult.Error!.Message, ErrorType.Validation);

            var deviceIdResult = EntityId.Create(deviceId);
            if (deviceIdResult.IsFailure)
                return Result<Telemetry>.Failure(deviceIdResult.Error!.Message, ErrorType.Validation);

            var nominalPowerResult = Power.Create(nominalPower);
            if (nominalPowerResult.IsFailure)
                return Result<Telemetry>.Failure(nominalPowerResult.Error!.Message, ErrorType.Validation);

            var currentPowerResult = Power.Create(currentPower);
            if (currentPowerResult.IsFailure)
                return Result<Telemetry>.Failure(currentPowerResult.Error!.Message, ErrorType.Validation);

            var firmwareResult = FirmwareVersion.Create(firmwareVersion);
            if (firmwareResult.IsFailure)
                return Result<Telemetry>.Failure(firmwareResult.Error!.Message, ErrorType.Validation);

            var telemetry = new Telemetry(
                idResult.Value,
                deviceIdResult.Value,
                deviceName,
                deviceType,
                nominalPowerResult.Value,
                currentPowerResult.Value,
                timestamp,
                firmwareResult.Value);

            return Result<Telemetry>.Success(telemetry);
        }

        #endregion
    }
}