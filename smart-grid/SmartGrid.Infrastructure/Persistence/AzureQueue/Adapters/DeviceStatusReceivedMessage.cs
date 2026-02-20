using SmartGrid.Application.Interfaces.Messaging;
using SmartGrid.Domain.Models;
using SmartGrid.Infrastructure.Persistence.AzureQueue.Messages;

namespace SmartGrid.Infrastructure.Persistence.AzureQueue.Adapters
{
    internal class DeviceStatusReceivedMessage(IReceivedMessage<DeviceStatusMessage> inner,
                                               DeviceStatus model
    ) : IReceivedMessage<DeviceStatus>
    {
        public DeviceStatus Body { get; } = model;

        public Task CompleteAsync() => inner.CompleteAsync();
    }
}
