using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Enums;
using SmartGrid.Domain.Models;
using SmartGrid.Domain.ValueObjects;
using SmartGrid.Infrastructure.Common.Options;
using SmartGrid.Infrastructure.Persistence.AzureTable.Common;
using SmartGrid.Infrastructure.Persistence.AzureTable.Entities;

namespace SmartGrid.Infrastructure.Persistence.AzureTable.Repositories
{
    internal class FirmwareRepository(
            TableServiceClient tableServiceClient,
            ITableKeyProvider<Firmware> keyProvider,
            ITableMapper<Firmware, FirmwareEntity> mapper,
            IOptions<AzureTableOptions> options
    ) : AzureTableRepository<Firmware, FirmwareEntity>(
              tableServiceClient.GetTableClient(options.Value.FirmwaresTable),
              keyProvider,
              mapper),
        IFirmwareRepository
    {
        public async Task SaveAsync(Firmware firmware, CancellationToken ct)
        {
           await base.AddAsync(firmware, ct);
        }
        public async Task<FirmwareVersion?> GetLatestVersionAsync(DeviceType deviceType, CancellationToken ct)
        {
            var firmwares = await base.QueryByPartitionKeyAsync(deviceType.ToString(), ct);

            var latest = firmwares
                    .Select(f => f.Version)
                    .OrderByDescending(v => v, Comparer<FirmwareVersion>.Create((a, b) => a.CompareTo(b)))
                    .FirstOrDefault();

            return latest;
        }
    }
}
