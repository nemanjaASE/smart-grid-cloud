using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;
using System.Net;

namespace SmartGrid.Functions.Middlewares;

internal class ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
    : IFunctionsWorkerMiddleware
{
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var exception = UnpackException(ex);
            var httpContext = context.GetHttpContext();

            if (httpContext != null)
            {
                await HandleGlobalErrorAsync(httpContext, context, exception);
            }
            else
            {
                logger.LogCritical(exception, "Background function {FunctionName} failed critically.",
                                 context.FunctionDefinition.Name);
                throw; // Tell Azure to retry
            }
        }
    }

    private async Task HandleGlobalErrorAsync(HttpContext httpContext,
                                              FunctionContext context,
                                              Exception exception)
    {
        var (statusCode, message) = exception switch
        {
            System.Text.Json.JsonException =>
                (HttpStatusCode.BadRequest, "The provided JSON is invalid."),
            InvalidOperationException =>
                (HttpStatusCode.BadRequest, exception.Message),

            _ =>
                (HttpStatusCode.InternalServerError, "An unexpected server error occurred.")
        };

        if (statusCode == HttpStatusCode.InternalServerError)
        {
            logger.LogError(exception, "[FATAL] Unhandled error in {FunctionName}. InvocationId: {Id}",
                          context.FunctionDefinition.Name, context.InvocationId);
        }
        else
        {
            logger.LogWarning("Global middleware caught a request error: {Message}", exception.Message);
        }

        httpContext.Response.StatusCode = (int)statusCode;
        httpContext.Response.ContentType = "application/json";

        var response = new
        {
            error = message,
            type = statusCode.ToString(),
            traceId = context.InvocationId
        };

        await httpContext.Response.WriteAsJsonAsync(response);
    }

    private static Exception UnpackException(Exception ex)
    {
        while (ex.InnerException != null &&
              (ex is AggregateException || ex is System.Reflection.TargetInvocationException))
        {
            ex = ex.InnerException;
        }
        return ex;
    }
}