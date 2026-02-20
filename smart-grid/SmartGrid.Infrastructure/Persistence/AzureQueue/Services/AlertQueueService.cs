using Azure.Storage.Queues;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartGrid.Application.Interfaces;
using SmartGrid.Application.Interfaces.Messaging;
using SmartGrid.Domain.ValueObjects;
using SmartGrid.Infrastructure.Common.Options;
using SmartGrid.Infrastructure.Persistence.AzureQueue.Mappers;
using SmartGrid.Infrastructure.Persistence.AzureQueue.Messages;

namespace SmartGrid.Infrastructure.Persistence.AzureQueue.Services
{
    internal class AlertQueueService(
        QueueServiceClient queueServiceClient,
        IJsonSerializer serializer,
        ILogger<AlertQueueService> logger,
        IOptions<AzureQueueOptions> options
    ) : AzureQueueService<AlertMessage>(
              queueServiceClient.GetQueueClient(options.Value.AlertQueue), 
              serializer,
              logger),
         IAlertQueueService
    {
        public async Task SendAlertAsync(Alert alert, CancellationToken ct = default)
        {
            var message = alert.ToQueueMessage();

            if (message != null)
            {
                await base.SendMessageAsync(message, ct);
            }
        }
    }
}
