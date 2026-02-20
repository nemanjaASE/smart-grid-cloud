namespace SmartGrid.Application.Interfaces.Messaging
{
    public interface IMessageQueueService<T>
    {
        Task SendMessageAsync(T message, CancellationToken ct = default);
        Task<IReceivedMessage<T>?> ReceiveMessageAsync(CancellationToken ct = default);
    }
}
