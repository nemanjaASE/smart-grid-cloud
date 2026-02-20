namespace SmartGrid.Application.Common.Extensions
{
    public static class FirmwarePathExtensions
    {
        public static string GetFirmwareBlobPath(FirmwareMetadata metadata)
        {
            return $"{metadata.DeviceType}/firmware_{metadata.Version}.bin";
        }

        public static FirmwareMetadata ParseMetadataFromPath(string blobName)
        {
            // Example: "solar-device/firmware_v2.0.bin"
            var parts = blobName.Split('/');
            var deviceType = parts[0];
            var version = parts[1]
                .Replace("firmware_", "")
                .Replace(".bin", "");

            return new FirmwareMetadata() { DeviceType = deviceType, Version = version };
        }
    }
}
