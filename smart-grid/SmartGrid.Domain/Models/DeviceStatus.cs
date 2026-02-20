using SmartGrid.Domain.Common;
using SmartGrid.Domain.Constants;
using SmartGrid.Domain.Enums;
using SmartGrid.Domain.ValueObjects;

namespace SmartGrid.Domain.Models
{
    public class DeviceStatus
    {
        public EntityId DeviceId { get; private set; }
        public DeviceType DeviceType { get; private set; } = DeviceType.Unknown;

        // SNAPSHOT
        public Power CurrentPower { get; private set; }
        public Percentage LoadPercentage { get; private set; }
        public DateTime LastHeartbeat { get; private set; }

        // STATUS
        public bool IsOnline(DateTime currentTime) => currentTime - LastHeartbeat < DeviceStatusLimits.OfflineThreshold;
        public bool IsUnderperforming => LoadPercentage < DeviceStatusLimits.UnderperformingLoad;
        public bool IsOverloaded => LoadPercentage > DeviceStatusLimits.OverloadedLoad;

        //FIRMWARE STASUS
        public FirmwareVersion CurrentFirmwareVersion { get; private set; }
        public FirmwareVersion? TargetFirmwareVersion { get; private set; }
        public UpdateStatus UpdateStatus { get; private set; } = UpdateStatus.UpToDate;

        private DeviceStatus(
            EntityId deviceId,
            DeviceType deviceType,
            Power currentPower,
            Percentage loadPercentage,
            DateTime lastHeartbeat,
            FirmwareVersion currentFirmwareVersion,
            UpdateStatus updateStatus,
            FirmwareVersion? targetFirmwareVersion = null)
        {
            DeviceId = deviceId;
            DeviceType = deviceType;
            CurrentPower = currentPower;
            LoadPercentage = loadPercentage;
            LastHeartbeat = lastHeartbeat;
            CurrentFirmwareVersion = currentFirmwareVersion;
            TargetFirmwareVersion = targetFirmwareVersion;
            UpdateStatus = updateStatus;
        }

        #region Factory Method

        public static DeviceStatus CreateDefault(EntityId deviceId, DeviceType deviceType, DateTime now)
        {
            return new DeviceStatus(
                deviceId,
                deviceType,
                Power.Create(0).Value,
                Percentage.Zero(),
                now,
                FirmwareVersion.Create("V1.0.0").Value,
                UpdateStatus.UpToDate
            );
        }
        public static Result<DeviceStatus> Load(
            string deviceId,
            DeviceType deviceType,
            double currentPower,
            double loadPercentage,
            DateTime lastHeartbeat,
            string currentFirmwareVersion,
            string? targetFirmwareVersion,
            UpdateStatus updateStatus)
        {
            var deviceIdResult = EntityId.Create(deviceId);
            if (deviceIdResult.IsFailure) 
                return Result<DeviceStatus>.Failure(deviceIdResult.Error!.Message, ErrorType.Validation);

            var powerResult = Power.Create(currentPower);
            if (powerResult.IsFailure)
                return Result<DeviceStatus>.Failure(powerResult.Error!.Message, ErrorType.Validation);

            var loadResult = Percentage.Create(loadPercentage);
            if (loadResult.IsFailure) 
                return Result<DeviceStatus>.Failure(loadResult.Error!.Message, ErrorType.Validation);

            var currentFwResult = FirmwareVersion.Create(currentFirmwareVersion);
            if (currentFwResult.IsFailure) 
                return Result<DeviceStatus>.Failure(currentFwResult.Error!.Message, ErrorType.Validation);

            FirmwareVersion? targetFw = null;
            if (!string.IsNullOrWhiteSpace(targetFirmwareVersion))
            {
                var targetFwResult = FirmwareVersion.Create(targetFirmwareVersion);
                if (targetFwResult.IsFailure)
                    return Result<DeviceStatus>.Failure(targetFwResult.Error!.Message, ErrorType.Validation);

                targetFw = targetFwResult.Value;
            }

            return Result<DeviceStatus>.Success(new DeviceStatus(
                deviceIdResult.Value,
                deviceType,
                powerResult.Value,
                Percentage.FromRaw(loadResult.Value),
                lastHeartbeat,
                currentFwResult.Value,
                updateStatus,
                targetFw
            ));
        }

        #endregion

        #region Domain Logic

        public void UpdateTelemetry(Telemetry telemetry)
        {
            LastHeartbeat = telemetry.Timestamp;
            CurrentPower = telemetry.CurrentPower;
            LoadPercentage= telemetry.LoadPercentage;

            if (UpdateStatus == UpdateStatus.PendingUpdate &&
                TargetFirmwareVersion != null &&
                telemetry.FirmwareVersion == TargetFirmwareVersion)
            {
                CurrentFirmwareVersion = telemetry.FirmwareVersion;
                UpdateStatus = UpdateStatus.UpToDate;
                TargetFirmwareVersion = null;
            }

        }

        public void SetTargetVersion(FirmwareVersion newTarget)
        {
            TargetFirmwareVersion = newTarget;

            if (CurrentFirmwareVersion.CompareTo(newTarget) < 0)
            {
                UpdateStatus = UpdateStatus.PendingUpdate;
            }
            else
            {
                UpdateStatus = UpdateStatus.UpToDate;
            }
        }
        public void InitializeFirmwareVersion(FirmwareVersion latestAvailableVersion)
        {
            if (latestAvailableVersion is null || string.IsNullOrWhiteSpace(latestAvailableVersion.Value))
                return;

            if (CurrentFirmwareVersion != latestAvailableVersion)
            {
                TargetFirmwareVersion = latestAvailableVersion;
                UpdateStatus = UpdateStatus.PendingUpdate;
            }
            else
            {
                UpdateStatus = UpdateStatus.UpToDate;
            }
        }
        public Alert? GetCurrentAnomaly(DateTime currentTime)
        {
            if (!IsOnline(currentTime))
            {
                return Alert.Create(
                    DeviceId,
                    AlertType.Critical,
                    $"Heartbeat missing. Device has been offline since {LastHeartbeat} UTC.").Value;
            }

            if (IsOverloaded)
            {
                return Alert.Create(
                    DeviceId,
                    AlertType.Critical,
                    $"Critical load detected: {LoadPercentage}%. Overload threshold exceeded.").Value;
            }

            if (IsUnderperforming)
            {
                return Alert.Create(
                    DeviceId,
                    AlertType.Warning,
                    $"Device is underperforming. Current load is only {LoadPercentage}.").Value;
            }

            return null;
        }

        #endregion
    }
}
