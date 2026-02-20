using SmartGrid.Domain.Models;

namespace SmartGrid.Application.Interfaces.Messaging
{
    public interface IDeviceStatusQueueService
    {
        Task SendStatusUpdateAsync(DeviceStatus status, CancellationToken ct = default);
        Task<IReceivedMessage<DeviceStatus>?> ReceiveStatusUpdateAsync(CancellationToken ct = default);
    }
}
