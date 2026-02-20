using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartGrid.WebApi.DTOs;
using SmartGrid.WebApi.Extensions;

namespace SmartGrid.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FirmwaresController(IMediator mediator) : ControllerBase
    {
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload([FromForm] UploadFirmwareRequest request, CancellationToken ct)
        {
            var commandResult = await request.ToCommandAsync(ct);

            if (commandResult.IsFailure)
                return commandResult.ToActionResult();

            var result = await mediator.Send(commandResult.Value, ct);
            return result.ToActionResult();
        }
    }
}
