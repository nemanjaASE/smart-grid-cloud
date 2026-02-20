using SmartGrid.Domain.Enums;
using SmartGrid.Domain.Models;
using SmartGrid.Infrastructure.Persistence.AzureTable.Common;
using SmartGrid.Infrastructure.Persistence.AzureTable.Entities;

namespace SmartGrid.Infrastructure.Persistence.AzureTable.Mappers
{
    internal class TelemetryTableMapper : ITableMapper<Telemetry, TelemetryEntity>
    {
        public TelemetryEntity ToEntity(Telemetry domain)
        {
            return new TelemetryEntity
            {
                DeviceName = domain.DeviceName,
                ObservationTime = domain.Timestamp,
                Load = domain.LoadPercentage,
                DeviceType = domain.DeviceType.ToString(),
                CurrentPower = domain.CurrentPower.Value,
                NominalPower = domain.NominalPower.Value,
                FirmwareVersion = domain.FirmwareVersion.Value
            };
        }

        public Telemetry? ToDomain(TelemetryEntity entity)
        {
            var type = Enum.TryParse<DeviceType>(entity.PartitionKey, out var parsedType)
                ? parsedType
                : DeviceType.Unknown;

            var parts = entity.RowKey.Split('_');
            var telemetryId = parts.Length > 1 ? parts[1] : entity.RowKey;

            var telemetryResult = Telemetry.Load(
                telemetryId,
                entity.PartitionKey,
                entity.DeviceName,
                type,
                entity.NominalPower,
                entity.CurrentPower,
                entity.ObservationTime,
                entity.FirmwareVersion
            );

            if (telemetryResult.IsFailure)
                return null;

            return telemetryResult.Value;
        }
    }
}
