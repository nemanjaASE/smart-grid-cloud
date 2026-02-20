using Microsoft.AspNetCore.Mvc;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;

namespace SmartGrid.Functions.Extensions
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
                ? new OkObjectResult(new
                {
                    message = "Operation successful.",
                    data = result.Value
                })
                : HandleFailure(result);
        }

         private static ObjectResult HandleFailure(Result result)
        {
            object messagePayload = result.Error?.Message ?? "An error occurred.";

            if (result.Error?.Type == ErrorType.Validation && !string.IsNullOrEmpty(result.Error.Message))
            {
                try
                {
                    var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var details = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string[]>>(result.Error.Message, options);

                    messagePayload = details!;
                }
                catch
                {
                    messagePayload = result.Error.Message;
                }
            }

            var errorResponse = new
            {
                error = result.Error?.Type.ToString(),
                details = messagePayload
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
