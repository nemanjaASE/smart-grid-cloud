using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Features.Firmwares.Commands;
using SmartGrid.Domain.Enums;

namespace SmartGrid.Functions.Processing;
internal class FirmwareProcessor(IMediator mediator, ILogger<FirmwareProcessor> logger)
{
    [Function("FirmwareUploadProcessor")]
    public async Task Run(
        [BlobTrigger("firmware-updates/{deviceType}/firmware_{version}.bin", 
        Connection = "AzureWebJobsStorage")] Stream firmwareStream,
        string deviceType,
        string version)
    {
        if (!Enum.TryParse<DeviceType>(deviceType, true, out var parsedDeviceType))
        {
            logger.LogError("[FIRMWARE] Invalid device type found in blob path: {DeviceType}", parsedDeviceType);
            return;
        }

        logger.LogInformation("[PROCESSOR] New firmware detected! Type: {DeviceType}, Version: {Version}", 
            deviceType, 
            version);

        var result = await mediator.Send(new ApplyTargetVersionCommand(parsedDeviceType, version));

        if (result.IsFailure)
        {
            logger.LogError("[PROCESSOR] Failed to apply firmware update to devices: {Error}", 
                result.Error?.Message);
        }
    }
}