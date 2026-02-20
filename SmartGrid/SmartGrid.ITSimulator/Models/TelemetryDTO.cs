namespace SmartGrid.ITSimulator.Models
{
    public class TelemetryDTO
    {
        public string DeviceId { get; set; } = string.Empty;
        public string DeviceName { get; set; } = string.Empty;
        public string DeviceType { get; set; } = string.Empty;
        public double NominalPower { get; set; }
        public double CurrentPower { get; set; }
        public string FirmwareVersion { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}