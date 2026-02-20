using MediatR;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Common;
using SmartGrid.Application.Interfaces.Messaging;
using SmartGrid.Domain.Events;

namespace SmartGrid.Application.Features.Alerts.EventHandlers
{
    internal class LogAlertEventHandler(
        IAlertQueueService alertQueueService,
        ILogger<LogAlertEventHandler> logger) : INotificationHandler<DomainEventNotification<AnomalyDetectedDomainEvent>>
    {
        public async Task Handle(DomainEventNotification<AnomalyDetectedDomainEvent> notification, CancellationToken ct)
        {
            var alert = notification.Event.Alert;

            try
            {
                await alertQueueService.SendAlertAsync(alert, ct);
                logger.LogInformation("[ALERT] Successfully sent alert to queue for Device: {DeviceId}", alert.DeviceId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[ALERT] Failed to send alert to queue for Device: {DeviceId}", alert.DeviceId);
            }
        }
    }
}
