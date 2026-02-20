using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using SmartGrid.Application.Features.Telemetries.Commands;
using SmartGrid.Domain.Common;
using SmartGrid.Functions.Extensions;

namespace SmartGrid.Functions.Ingestion;
internal class ReceiveTelemetry(IMediator mediator)
{
    [Function("ReceiveTelemetry")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
    {
        ProcessTelemetryCommand? command = await req.ReadFromJsonAsync<ProcessTelemetryCommand>();

        if (command == null) return new BadRequestObjectResult(new { error = "Invalid or empty JSON payload." });

        Result result = await mediator.Send(command);

        return result.ToActionResult();
    }
}