using SmartGrid.ITSimulator.Enums;
using SmartGrid.ITSimulator.Models;

namespace SmartGrid.ITSimulator.Services
{
    public class SimulatorService
    {
        private readonly Random _random;
        private readonly double _maxPowerVariation;

        public SimulatorService(double maxPowerVariation = 50)
        {
            _random = new Random();
            _maxPowerVariation = maxPowerVariation;
        }

        public TelemetryDTO GenerateTelemetry(string deviceId, 
                                              string deviceName, 
                                              double nominalPower, 
                                              string firmwareVersion, 
                                              DeviceType deviceType)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
                throw new ArgumentException("Device Id cannot be empty", nameof(deviceName));

            if (string.IsNullOrWhiteSpace(deviceName))
                throw new ArgumentException("Device Name cannot be empty", nameof(deviceName));

            if (string.IsNullOrWhiteSpace(firmwareVersion))
                throw new ArgumentException("Firmware Version cannot be empty", nameof(firmwareVersion));

            if (nominalPower <= 0)
                throw new ArgumentException("Nominal power must be greater than zero", nameof(nominalPower));

            double currentPower = _random.NextDouble() * (nominalPower + _maxPowerVariation);

            return new TelemetryDTO
            {
                DeviceId = deviceId,
                DeviceName = deviceName,
                Timestamp = DateTime.UtcNow,
                NominalPower = nominalPower,
                CurrentPower = currentPower,
                FirmwareVersion = firmwareVersion,
                DeviceType = deviceType.ToString()
            };
        }
    }
}
