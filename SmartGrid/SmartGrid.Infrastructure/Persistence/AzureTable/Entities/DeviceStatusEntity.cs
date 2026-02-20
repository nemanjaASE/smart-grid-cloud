namespace SmartGrid.Infrastructure.Persistence.AzureTable.Entities
{
    internal class DeviceStatusEntity : BaseTableEntity
    {
        public double CurrentPower { get; set; }
        public double LoadPercentage { get; set; }
        public DateTime LastHeartbeat { get; set; }

        public string CurrentFirmwareVersion { get; set; } = string.Empty;
        public string? TargetFirmwareVersion { get; set; } = string.Empty;
        public string UpdateStatus { get; set; } = string.Empty;
    }
}