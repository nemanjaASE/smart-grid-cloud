using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using SmartGrid.Application.Interfaces;
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
    internal class DeviceRepository(
        TableServiceClient tableServiceClient,
        ITableKeyProvider<Device> deviceKeyProvider,
        ITableKeyProvider<DeviceStatus> deviceStatusKeyProvider,
        ITableMapper<Device, DeviceEntity> deviceMapper,
        ITableMapper<DeviceStatus, DeviceStatusEntity> statusMapper,
        IOptions<AzureTableOptions> options,
        IDateTimeProvider dateTimeProvider)
        : AzureTableRepository<Device, DeviceEntity>(
            tableServiceClient.GetTableClient(options.Value.DevicesTable),
            deviceKeyProvider,
            deviceMapper), IDeviceRepository
    {
        private readonly TableClient _statusTableClient = tableServiceClient.GetTableClient(options.Value.DeviceStatusesTable);
        private readonly ITableMapper<DeviceStatus, DeviceStatusEntity> _statusMapper = statusMapper;

        public async Task<IReadOnlyCollection<Device>> GetAllDevicesAsync(CancellationToken ct = default)
        {
            return await base.QueryAsync(string.Empty, ct);
        }
        public async Task<Device?> GetByIdAsync(DeviceType deviceType, EntityId deviceId, CancellationToken ct = default)
        {
            return await base.GetByIdAsync(deviceType.ToString(), deviceId, ct);
        }
        public async Task<Device?> GetWithStatusByIdAsync(DeviceType deviceType, EntityId deviceId, CancellationToken ct = default)
        {
            var partitionKey = deviceType.ToString();
            var rowKey = deviceId.Value;

            var deviceTask = base.GetByIdAsync(partitionKey, deviceId, ct);

            var statusTask = _statusTableClient.GetEntityIfExistsAsync<DeviceStatusEntity>(
                partitionKey,
                rowKey,
                cancellationToken: ct);

            await Task.WhenAll(deviceTask, statusTask);

            var device = deviceTask.Result;
            if (device is null) return null;

            var statusResult = statusTask.Result;
            var status = (statusResult.HasValue && statusResult.Value is not null)
                ? _statusMapper.ToDomain(statusResult.Value) ?? DeviceStatus.CreateDefault(device.Id, device.Type, dateTimeProvider.UtcNow)
                : DeviceStatus.CreateDefault(device.Id, device.Type, dateTimeProvider.UtcNow);

            var loadResult = Device.Load(
                device.Id.Value,
                device.Type,
                device.Name,
                device.NominalPower.Value,
                device.Location,
                device.RegisteredAt,
                status
            );

            return loadResult.IsSuccess ? loadResult.Value : null;
        }
        public async Task<IReadOnlyCollection<Device>> GetAllAsync(CancellationToken ct = default)
        {
            return await base.QueryAsync(string.Empty, ct);
        }
        public async IAsyncEnumerable<Device> GetAllWithStatusStreamingAsync([EnumeratorCancellation] CancellationToken ct = default)
        {
            var deviceEntities = base._tableClient.QueryAsync<DeviceEntity>(cancellationToken: ct);

            await foreach (var entity in deviceEntities.WithCancellation(ct))
            {
                var device = _mapper.ToDomain(entity);
                if (device is null) continue;

                var status = await GetStatusOrDefaultAsync(device.Type, device.Id, ct);

                var loadResult = Device.Load(
                    device.Id.Value, device.Type, device.Name, device.NominalPower.Value,
                    device.Location, device.RegisteredAt, status
                );

                if (loadResult.IsSuccess)
                    yield return loadResult.Value;
            }
        }
        public async Task<IReadOnlyCollection<Device>> GetAllWithStatusByTypeAsync(DeviceType type, CancellationToken ct = default)
        {
            var result = new List<Device>();
            var query = _statusTableClient.QueryAsync<DeviceStatusEntity>(
                s => s.PartitionKey == type.ToString(),
                cancellationToken: ct);

            await foreach (var entity in query)
            {
                var status = _statusMapper.ToDomain(entity);
                if (status is null)
                    continue;

                var device = await base.GetByIdAsync(type.ToString(), status.DeviceId, ct);
                if (device is null)
                    continue;

                var loadResult = Device.Load(
                    device.Id.Value,
                    device.Type,
                    device.Name,
                    device.NominalPower.Value,
                    device.Location,
                    device.RegisteredAt,
                    status
                );

                if (loadResult.IsSuccess)
                    result.Add(loadResult.Value);
            }

            return result;
        }

        public async Task SaveAsync(Device device, CancellationToken ct = default)
        {
            await base.AddAsync(device, ct);

            var statusEntity = _statusMapper.ToEntity(device.Status);

            statusEntity.PartitionKey = deviceStatusKeyProvider.GetPartitionKey(device.Status);
            statusEntity.RowKey = deviceStatusKeyProvider.GetRowKey(device.Status);

            await _statusTableClient.UpsertEntityAsync(statusEntity, TableUpdateMode.Replace, ct);
        }
        public async Task SaveStatusAsync(DeviceStatus status, CancellationToken ct = default)
        {
            var statusEntity = _statusMapper.ToEntity(status);

            statusEntity.PartitionKey = deviceStatusKeyProvider.GetPartitionKey(status);
            statusEntity.RowKey = deviceStatusKeyProvider.GetRowKey(status);

            await _statusTableClient.UpsertEntityAsync(statusEntity, TableUpdateMode.Replace, ct);
        }

        private async Task<DeviceStatus> GetStatusOrDefaultAsync(DeviceType type, EntityId deviceId, CancellationToken ct)
        {
            var response =
                await _statusTableClient.GetEntityIfExistsAsync<DeviceStatusEntity>(type.ToString(), deviceId.Value, cancellationToken: ct);

            if (response.HasValue && response.Value is not null)
                return _statusMapper.ToDomain(response.Value) ?? DeviceStatus.CreateDefault(deviceId, type, dateTimeProvider.UtcNow);

            return DeviceStatus.CreateDefault(deviceId, type, dateTimeProvider.UtcNow);
        }
    }
}