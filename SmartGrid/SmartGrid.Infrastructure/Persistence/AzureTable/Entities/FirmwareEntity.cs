namespace SmartGrid.Infrastructure.Persistence.AzureTable.Entities
{
    internal class FirmwareEntity : BaseTableEntity
    {
        public string Version { get; set; } = default!;
        public string FileName { get; set; } = default!;
        public long FileSizeInBytes { get; set; } = default!;
        public DateTime UploadedAt { get; set; }
    }
}
