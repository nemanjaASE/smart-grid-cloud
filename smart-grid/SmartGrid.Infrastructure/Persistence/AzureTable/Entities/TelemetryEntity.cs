namespace SmartGrid.Infrastructure.Persistence.AzureTable.Entities
{
    internal class TelemetryEntity : BaseTableEntity
    {
        public string DeviceName { get; set; } = default!;
        public string DeviceType { get; set; } = default!;
        public double NominalPower { get; set; }
        public double CurrentPower { get; set; }
        public double Load { get; set; }
        public string FirmwareVersion { get; set; } = default!;
        public DateTime ObservationTime { get; set; }
    }
}
