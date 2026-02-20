using SmartGrid.Domain.Enums;
using SmartGrid.Domain.Models;
using SmartGrid.Infrastructure.Persistence.AzureTable.Common;
using SmartGrid.Infrastructure.Persistence.AzureTable.Entities;

namespace SmartGrid.Infrastructure.Persistence.AzureTable.Mappers
{
    internal class FirmwareTableMapper : ITableMapper<Firmware, FirmwareEntity>
    {
        public FirmwareEntity ToEntity(Firmware domain)
        {
            return new FirmwareEntity
            {
                Version = domain.Version.Value,
                FileName = domain.FileName.Value,
                FileSizeInBytes = domain.FileSizeInBytes,
                UploadedAt = domain.UploadedAt
            };
        }
        public Firmware? ToDomain(FirmwareEntity entity)
        {
            var type = Enum.TryParse<DeviceType>(entity.PartitionKey, out var parsedType)
               ? parsedType
               : DeviceType.Unknown;

            var firmwareResult = Firmware.Load(
                entity.RowKey,
                type,
                entity.Version,
                entity.FileName,
                entity.FileSizeInBytes,
                entity.UploadedAt
            );

           if (firmwareResult.IsFailure)
                return null;

            return firmwareResult.Value;
        }
    }
}
