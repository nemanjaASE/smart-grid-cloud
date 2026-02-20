using SmartGrid.Domain.Models;
using SmartGrid.Infrastructure.Persistence.AzureTable.Common;

namespace SmartGrid.Infrastructure.Persistence.AzureTable.KeyProviders
{
    internal class DeviceStatusTableKeyProvider : ITableKeyProvider<DeviceStatus>
    {
        public string GetPartitionKey(DeviceStatus model)
        {
            return model.DeviceType.ToString();
        }

        public string GetRowKey(DeviceStatus model)
        {
            return model.DeviceId.Value;
        }
    }
}
