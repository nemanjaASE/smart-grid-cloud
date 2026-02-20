using SmartGrid.Application.Interfaces;
using SmartGrid.Domain.Enums;
using SmartGrid.Domain.Models;
using SmartGrid.Domain.ValueObjects;
using SmartGrid.Infrastructure.Persistence.AzureTable.Common;
using SmartGrid.Infrastructure.Persistence.AzureTable.Entities;

namespace SmartGrid.Infrastructure.Persistence.AzureTable.Mappers
{
    internal sealed class DeviceTableMapper(IDateTimeProvider dateTimeProvider) : ITableMapper<Device, DeviceEntity>
    {
        public DeviceEntity ToEntity(Device domain)
        {
            return new DeviceEntity
            {
                Name = domain.Name,
                Location = domain.Location,
                NominalPower = domain.NominalPower.Value,
                RegisteredAt = domain.RegisteredAt
            };
        }

        public Device? ToDomain(DeviceEntity entity)
        {
            var type = Enum.TryParse<DeviceType>(entity.PartitionKey, out var parsedType)
               ? parsedType
               : DeviceType.Unknown;

            var defaultStatus = DeviceStatus.CreateDefault(
                EntityId.Create(entity.RowKey).Value,
                type,
                dateTimeProvider.UtcNow);

            var deviceResult = Device.Load(
                entity.RowKey,
                type,
                entity.Name,
                entity.NominalPower,
                entity.Location,
                entity.RegisteredAt,
                defaultStatus
            );

            if (deviceResult.IsFailure)
                return null;

            return deviceResult.Value;
        }
    }
}
