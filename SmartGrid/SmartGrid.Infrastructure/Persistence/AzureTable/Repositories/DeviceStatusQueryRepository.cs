using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Enums;
using SmartGrid.Domain.Models;
using SmartGrid.Domain.ValueObjects;
using SmartGrid.Infrastructure.Common.Options;
using SmartGrid.Infrastructure.Persistence.AzureTable.Common;
using SmartGrid.Infrastructure.Persistence.AzureTable.Entities;
using System.Runtime.CompilerServices;

namespace SmartGrid.Infrastructure.Persistence.AzureTable.Repositories
{
    internal class DeviceStatusQueryRepository(
        TableServiceClient tableServiceClient,
        ITableKeyProvider<DeviceStatus> deviceStatusKeyProvider,
        ITableMapper<DeviceStatus, DeviceStatusEntity> statusMapper,
        IOptions<AzureTableOptions> options)
        : AzureTableRepository<DeviceStatus, DeviceStatusEntity>(
            tableServiceClient.GetTableClient(options.Value.DeviceStatusesTable),
            deviceStatusKeyProvider,
            statusMapper), IDeviceStatusQueryRepository
    {
        public async Task<IReadOnlyCollection<DeviceStatus>> GetAllAsync(CancellationToken ct = default)
        {
            return await base.QueryAsync(string.Empty, ct);
        }

        public async Task<DeviceStatus?> GetByIdAsync(DeviceType type, EntityId deviceId, CancellationToken ct = default)
        {
            return await base.GetByIdAsync(type.ToString(), deviceId, ct);
        }

        public async Task<IReadOnlyCollection<DeviceStatus>> GetByTypeAsync(DeviceType type, CancellationToken ct = default)
        {
            return await base.QueryByPartitionKeyAsync(type.ToString(), ct);
        }

        public async IAsyncEnumerable<DeviceStatus> GetByTypeStreamingAsync(DeviceType type,[EnumeratorCancellation] CancellationToken ct = default)
        {
            var partitionKey = type.ToString();

            var query = _tableClient.QueryAsync<DeviceStatusEntity>(
                filter: $"PartitionKey eq '{partitionKey}'",
                cancellationToken: ct);

            await foreach (var entity in query.WithCancellation(ct))
            {
                var domainModel = base._mapper.ToDomain(entity);

                if (domainModel is null)
                    continue;

                yield return domainModel;
            }
        }
    }
}
