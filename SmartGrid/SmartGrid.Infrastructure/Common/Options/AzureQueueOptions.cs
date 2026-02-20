namespace SmartGrid.Infrastructure.Common.Options
{
    internal class AzureQueueOptions
    {
        public string ConnectionString { get; init; } = string.Empty;
        public string AlertQueue { get; init; } = string.Empty;
        public string DeviceStatusQueue {  get; init; } = string.Empty;
    }
}
