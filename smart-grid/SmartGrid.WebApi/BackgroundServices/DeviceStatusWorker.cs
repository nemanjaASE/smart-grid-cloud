using Microsoft.AspNetCore.SignalR;
using SmartGrid.Application.Features.DeviceStatuses.Queries;
using SmartGrid.Application.Interfaces;
using SmartGrid.Application.Interfaces.Messaging;
using SmartGrid.Domain.Models;
using SmartGrid.WebApi.Hubs;

namespace SmartGrid.WebApi.BackgroundServices
{
    internal class DeviceStatusWorker(
        IServiceProvider serviceProvider,
        IHubContext<DeviceHub> hubContext,
        IMapper<DeviceStatus, DeviceStatusDto> deviceStatusMapper,
        ILogger<DeviceStatusWorker> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("[WORKER] Device Status Worker started listening...");

            while (!stoppingToken.IsCancellationRequested)
            {
                bool foundMessage = false;

                try
                {
                    using var scope = serviceProvider.CreateScope();

                    var queueService = scope.ServiceProvider.GetRequiredService<IDeviceStatusQueueService>();

                    var message = await queueService.ReceiveStatusUpdateAsync(stoppingToken);

                    if (message != null)
                    {
                        foundMessage = true;
                        var deviceStatus = message.Body;
                        
                        logger.LogInformation("[WORKER] Received update for device: {DeviceId}",
                            deviceStatus.DeviceId);

                        var deviceStatusDto = deviceStatusMapper.Map(deviceStatus);

                        await hubContext.Clients.All.SendAsync("ReceiveStatusUpdate",
                                                               deviceStatusDto,
                                                               stoppingToken);

                        await message.CompleteAsync();

                        logger.LogInformation("[WORKER] Update broadcasted to clients and message deleted.");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "[ERROR] Error processing device status queue.");
                }

                if(!foundMessage)
                {
                    await Task.Delay(2000, stoppingToken);
                }
            }
        }
    }
}

