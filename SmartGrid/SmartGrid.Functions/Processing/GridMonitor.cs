using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Features.DeviceStatuses.Commands;

namespace SmartGrid.Functions.Processing;

internal class GridMonitor(ILogger<GridMonitor> logger, IMediator mediator)
{
    [Function("GridMonitor")]
    public async Task Run([TimerTrigger("%GridMonitorSchedule%")] TimerInfo myTimer)
    {

        logger.LogInformation("[TIMER] Starting grid monitoring cycle...");

        var result = await mediator.Send(new ExecuteMonitoringCommand());

        if (result.IsFailure)
        {
            logger.LogError("[MONITORING] Grid monitoring cycle failed: {errorMessage}", 
                result.Error?.Message);
        }
        else
        {
            logger.LogInformation("[MONITORING] Grid monitoring cycle completed successfully.");
        }

        if (myTimer.ScheduleStatus is not null)
        {
            logger.LogInformation("[TIMER] Next timer schedule at: {nextSchedule}", 
                myTimer.ScheduleStatus.Next);
        }
    }
}