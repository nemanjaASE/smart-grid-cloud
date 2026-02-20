using SmartGrid.Domain.Models;
using SmartGrid.Infrastructure.Persistence.AzureTable.Common;

namespace SmartGrid.Infrastructure.Persistence.AzureTable.KeyProviders
{
    internal class FirmwareTableKeyProvider : ITableKeyProvider<Firmware>
    {
        public string GetPartitionKey(Firmware model)
        {
            return model.DeviceType.ToString();
        }

        public string GetRowKey(Firmware model)
        {
            return model.Id.Value;
        }
    }
}
