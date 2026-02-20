using SmartGrid.Domain.Enums;
using SmartGrid.Domain.Models;
using SmartGrid.Domain.ValueObjects;

namespace SmartGrid.Application.Interfaces.Repositories
{
    public interface IDeviceStatusQueryRepository
    {
        Task<IReadOnlyCollection<DeviceStatus>> GetAllAsync(CancellationToken ct = default);
        Task<IReadOnlyCollection<DeviceStatus>> GetByTypeAsync(DeviceType type, CancellationToken ct = default);
        IAsyncEnumerable<DeviceStatus> GetByTypeStreamingAsync(DeviceType type, CancellationToken ct = default);
        Task<DeviceStatus?> GetByIdAsync(DeviceType type, EntityId deviceId, CancellationToken ct = default);
    }
}
