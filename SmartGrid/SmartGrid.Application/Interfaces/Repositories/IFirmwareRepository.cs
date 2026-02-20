using SmartGrid.Domain.Enums;
using SmartGrid.Domain.Models;
using SmartGrid.Domain.ValueObjects;

namespace SmartGrid.Application.Interfaces.Repositories
{
    public interface IFirmwareRepository
    {
        Task<FirmwareVersion?> GetLatestVersionAsync(DeviceType deviceType, CancellationToken ct = default);
        Task SaveAsync(Firmware firmware, CancellationToken ct = default);
    }
}
