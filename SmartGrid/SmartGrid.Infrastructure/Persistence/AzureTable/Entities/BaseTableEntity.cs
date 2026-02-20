using Azure;
using Azure.Data.Tables;

namespace SmartGrid.Infrastructure.Persistence.AzureTable.Entities
{
    internal class BaseTableEntity : ITableEntity
    {
        public string PartitionKey { get; set; } = default!;
        public string RowKey { get; set; } = default!;
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
