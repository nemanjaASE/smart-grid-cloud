using Azure.Storage.Queues;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Interfaces;
using SmartGrid.Application.Interfaces.Messaging;

namespace SmartGrid.Infrastructure.Persistence.AzureQueue
{
    internal abstract class AzureQueueService<T>(
        QueueClient queueClient,
        IJsonSerializer serializer,
        ILogger logger
    ) : IMessageQueueService<T> where T : class
    {
        protected readonly QueueClient _queueClient = queueClient;
        protected readonly IJsonSerializer _serializer = serializer;
        protected readonly ILogger _logger = logger;

        public async Task<IReceivedMessage<T>?> ReceiveMessageAsync(CancellationToken ct = default)
        {
            try
            {
                var response = await _queueClient.ReceiveMessagesAsync(maxMessages: 1,
                                                                       visibilityTimeout: TimeSpan.FromSeconds(30),
                                                                       cancellationToken: ct);
                var message = response.Value.FirstOrDefault();

                if (message == null) return null;

                var body = _serializer.Deserialize<T>(message.MessageText);

                if (body == null) return null;

                return new AzureReceivedMessage<T>(
                    _queueClient,
                    message.MessageId,
                    message.PopReceipt,
                    body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                                "Error receiving message from queue {QueueName}",
                                _queueClient.Name);
                return null;
            }
        }

        public async Task SendMessageAsync(T message, CancellationToken ct = default)
        {
            if (message == null) return;

            try
            {
                var json = _serializer.Serialize(message);

                await _queueClient.SendMessageAsync(json, ct);

                _logger.LogInformation(
                    "Message {MessageType} sent to queue {QueueName}",
                    typeof(T).Name,
                    _queueClient.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message to queue {QueueName}",
                                _queueClient.Name);
            }
        }
    }
}
