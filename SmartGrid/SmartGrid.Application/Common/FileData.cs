namespace SmartGrid.Application.Common
{
    public class FileData<TMetadata>
    {
        public TMetadata Metadata { get; set; } = default!;
        public byte[] Content { get; set; } = [];
    }
}
