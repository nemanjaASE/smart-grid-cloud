using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Features.Alerts.Commands;
using SmartGrid.Infrastructure.Persistence.AzureQueue.Mappers;
using SmartGrid.Infrastructure.Persistence.AzureQueue.Messages;

namespace SmartGrid.Functions.Processing;

internal class ProcessAlerts(ILogger<ProcessAlerts> logger, IMediator mediator)
{
    [Function(nameof(ProcessAlerts))]
    public async Task RunAsync(
        [QueueTrigger("%AzureQueueOptions:AlertQueue%", Connection = "AzureWebJobsStorage")] AlertMessage alertDto)
    {
        try
        {
            var alert = alertDto.ToDomainModel();

            if (alert == null)
            {
                logger.LogWarning("[ALERT PR] Failed alert mappings.");
                return;
            }

            var result = await mediator.Send(new ProcessAlertCommand(alert));


            if (!result.IsSuccess)
            {
                logger.LogWarning("[ALERT PR] Command failed: {Error}", result.Error);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[ALERT PR] Error while processing alert message.");
            throw;
        }
    }
}