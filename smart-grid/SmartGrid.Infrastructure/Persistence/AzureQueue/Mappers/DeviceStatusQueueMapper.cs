using SmartGrid.Domain.Models;
using SmartGrid.Infrastructure.Persistence.AzureQueue.Messages;

namespace SmartGrid.Infrastructure.Persistence.AzureQueue.Mappers
{
    public static class DeviceStatusQueueMapper
    {
        public static DeviceStatusMessage? ToQueueMessage(this DeviceStatus model)
        {
            if (model == null) return null;

            return new DeviceStatusMessage
            {
                DeviceId = model.DeviceId.Value,
                DeviceType = model.DeviceType,
                CurrentPower = model.CurrentPower.Value,
                LoadPercentage = model.LoadPercentage,
                LastHeartbeat = model.LastHeartbeat,
                CurrentFirmwareVersion = model.CurrentFirmwareVersion.Value,
                TargetFirmwareVersion = model.TargetFirmwareVersion?.Value,
                UpdateStatus = model.UpdateStatus
            };
        }

        public static DeviceStatus? ToDomainModel(this DeviceStatusMessage message)
        {
            var deviceStatusResult = DeviceStatus.Load(
                message.DeviceId,
                message.DeviceType,
                message.CurrentPower,
                message.LoadPercentage,
                message.LastHeartbeat,
                message.CurrentFirmwareVersion,
                message.TargetFirmwareVersion,
                message.UpdateStatus
            );

            if (deviceStatusResult.IsFailure)
                return null;

            return deviceStatusResult.Value;
        }
    }
}
