using SmartGrid.Domain.Models;
using SmartGrid.Infrastructure.Persistence.AzureTable.Common;

namespace SmartGrid.Infrastructure.Persistence.AzureTable.KeyProviders
{
    internal class DeviceTableKeyProvider : ITableKeyProvider<Device>
    {
        public string GetPartitionKey(Device model)
        {
            return model.Type.ToString();
        }

        public string GetRowKey(Device model)
        {
            return model.Id.Value;
        }
    }
}
