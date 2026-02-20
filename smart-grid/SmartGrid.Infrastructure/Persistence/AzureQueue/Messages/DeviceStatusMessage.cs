using SmartGrid.Domain.Enums;

namespace SmartGrid.Infrastructure.Persistence.AzureQueue.Messages
{
    public class DeviceStatusMessage
    {
        public string DeviceId { get; set; } = string.Empty;
        public DeviceType DeviceType { get; set; }
        // SNAPSHOT
        public double CurrentPower { get; set; }
        public double LoadPercentage { get; set; }
        // STATUS
        public DateTime LastHeartbeat { get; set; }
        //FIRMWARE STATUS
        public string CurrentFirmwareVersion { get; set; } = string.Empty;
        public string? TargetFirmwareVersion { get; set; } = string.Empty;
        public UpdateStatus UpdateStatus { get; set; } 
    }
}
