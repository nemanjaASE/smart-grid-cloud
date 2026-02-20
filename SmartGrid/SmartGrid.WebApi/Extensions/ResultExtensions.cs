using Microsoft.AspNetCore.Mvc;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;
using System.Text.Json;

namespace SmartGrid.WebApi.Extensions
{
    internal static class ResultExtensions
    {
        public static IActionResult ToActionResult(this Result result)
        {
            return result.IsSuccess
                            ? new OkObjectResult(new { message = "Operation successful." })
                            : HandleFailure(result);
        }

        public static IActionResult ToActionResult<T>(this Result<T> result)
        {
            return result.IsSuccess
                            ? new OkObjectResult(result.Value)
                            : HandleFailure(result);
        }

        private static ObjectResult HandleFailure(Result result)
        {
            object finalMessage = result.Error?.Message ?? "An error occurred.";

            if (result.Error?.Type == ErrorType.Validation && !string.IsNullOrWhiteSpace(result.Error.Message))
            {
                try
                {
                    var errors = JsonSerializer.Deserialize<Dictionary<string, string[]>>(result.Error.Message);
                    finalMessage = errors!;
                }
                catch
                {
                    finalMessage = result.Error.Message;
                }
            }

            var errorResponse = new
            {
                type = result.Error?.Type.ToString(),
                errors = result.Error?.Type == ErrorType.Validation ? finalMessage : null,
                message = result.Error?.Type != ErrorType.Validation ? finalMessage : "Validation failed."
            };

            return result.Error?.Type switch
            {
                ErrorType.Validation => new BadRequestObjectResult(errorResponse),
                ErrorType.NotFound => new NotFoundObjectResult(errorResponse),
                ErrorType.Unexpected => new ObjectResult(errorResponse) { StatusCode = 500 },
                _ => new BadRequestObjectResult(errorResponse)
            };
        }
    }
}
