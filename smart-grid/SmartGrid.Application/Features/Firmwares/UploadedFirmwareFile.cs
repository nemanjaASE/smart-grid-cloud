namespace SmartGrid.Application.Features.Firmwares
{
    public record UploadedFirmwareFile
    {
        public string FileName { get; init; } = string.Empty;
        public byte[] Content { get; init; } = Array.Empty<byte>();
    }
}