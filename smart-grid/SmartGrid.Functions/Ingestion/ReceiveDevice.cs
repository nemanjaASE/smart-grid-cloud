using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using SmartGrid.Application.Features.Devices.Commands;
using SmartGrid.Domain.Common;
using SmartGrid.Functions.Extensions;

namespace SmartGrid.Functions.Ingestion;

internal class ReceiveDevice(IMediator mediator)
{
    [Function("ReceiveDevice")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
    {
        AddNewDeviceCommand? command = await req.ReadFromJsonAsync<AddNewDeviceCommand>();

        if (command == null) return new BadRequestObjectResult(new { error = "Invalid or empty JSON payload." });

        Result<string> result = await mediator.Send(command);

        return result.ToActionResult();
    }
}