using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;
using SmartGrid.Domain.Events;
using SmartGrid.Domain.ValueObjects;

namespace SmartGrid.Domain.Models
{
    // AGGREGATE ROOT
    public class Device : AggregateRoot
    {
        public EntityId Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public DeviceType Type { get; private set; }
        public string Location { get; private set; } = string.Empty;
        public Power NominalPower { get; private set; }
        public DateTime RegisteredAt { get; private set; }

        // CHILD ENTITY
        public DeviceStatus Status { get; private set; } = null!;

        private Device(
            EntityId id,
            DeviceType type,
            string name,
            Power nominalPower,
            string location,
            DateTime registredAt)
        {
            Id = id;
            Type = type;
            Name = name;
            NominalPower = nominalPower;
            Location = location;
            RegisteredAt = registredAt;
        }

        #region Factory Methods

        public static Result<Device> Create(
            DeviceType type,
            string name,
            double nominalPower,
            string location,
            DateTime registeredAt)
        {
            if (!Enum.IsDefined(typeof(DeviceType), type) || type == DeviceType.Unknown)
                return Result<Device>.Failure("Invalid Device Type.", ErrorType.Validation);
            
            if (string.IsNullOrWhiteSpace(name))
                return Result<Device>.Failure("Name is required.", ErrorType.Validation);
            
            if (string.IsNullOrWhiteSpace(location))
                return Result<Device>.Failure("Location is required.", ErrorType.Validation);

            var powerResult = Power.Create(nominalPower);
            if (powerResult.IsFailure)
                return Result<Device>.Failure(powerResult.Error!.Message, ErrorType.Validation);

            var device = new Device(EntityId.New(), type, name, powerResult.Value, location, registeredAt);

            device.Status = DeviceStatus.CreateDefault(device.Id, device.Type, registeredAt);

            return Result<Device>.Success(device);
        }
        public static Result<Device> Load(
            string id,
            DeviceType type,
            string name,
            double nominalPower,
            string location,
            DateTime registeredAt,
            DeviceStatus status)
        {
            var idResult = EntityId.Create(id);

            if(idResult.IsFailure)
                return Result<Device>.Failure(idResult.Error!.Message, ErrorType.Validation);

            var powerResult = Power.Create(nominalPower);
            if (powerResult.IsFailure)
                return Result<Device>.Failure(powerResult.Error!.Message, ErrorType.Validation);

            var device = new Device(idResult.Value, type, name, powerResult.Value, location, registeredAt)
            {
                Status = status
            };

            return Result<Device>.Success(device);
        }

        #endregion

        #region Domain Logic

        public void ProcessTelemetry(Telemetry telemetry)
        {
            Status.UpdateTelemetry(telemetry);
        }

        public void ApplyInitialFirmware(FirmwareVersion latestVersion)
        {
            Status.InitializeFirmwareVersion(latestVersion);
        }

        public void EvaluateMonitoring(DateTime currentTime)
        {
            var alert = Status.GetCurrentAnomaly(currentTime);

            if (alert != null)
            {
                AddDomainEvent(new AnomalyDetectedDomainEvent(alert, currentTime));
            }
        }

        #endregion
    }
}
