using MediatR;
using Microsoft.Extensions.Logging;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;
using SmartGrid.Domain.ValueObjects;

namespace SmartGrid.Application.Features.Alerts.Commands
{
    // COMMAND
    public record ProcessAlertCommand(Alert Alert) : IRequest<Result>;

    // HANDLER
    internal class ProcessAlertHandler(ILogger<ProcessAlertHandler> logger) 
        : IRequestHandler<ProcessAlertCommand, Result>
    {
        public async Task<Result> Handle(ProcessAlertCommand request, CancellationToken ct)
        {
            var alert = request.Alert;
            string logMsg = $"[ALARM] {alert.AlertType} on {alert.DeviceId}: {alert.Message}";

            await Task.CompletedTask; // Simulate async work

            if (alert.AlertType == AlertType.Critical)
                logger.LogError(logMsg);
            else
                logger.LogWarning(logMsg);

            return Result.Success();
        }
    }
}