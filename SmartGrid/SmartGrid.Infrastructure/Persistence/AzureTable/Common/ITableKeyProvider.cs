namespace SmartGrid.Infrastructure.Persistence.AzureTable.Common
{
    internal interface ITableKeyProvider<T>
    {
        string GetPartitionKey(T model);
        string GetRowKey(T model);
    }
}
