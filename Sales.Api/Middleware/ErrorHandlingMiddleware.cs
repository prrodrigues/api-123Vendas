using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Sales.Domain.Exceptions;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sales.Api.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await HandleExceptionAsync(context, ex, context.Request.Path);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception, string path)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        string errorCode = "GENERIC_ERROR";
        string message = $"Ocorreu um erro ao processar a operação {path}.";

        if (exception is AppException appEx)
        {
            errorCode = appEx.ErrorCode;
            message = appEx.Message;
        }
        else if (exception is ArgumentException argEx)
        {
            errorCode = "ARGUMENT_ERROR";
            message = argEx.Message;
        }

        var errorResponse = new
        {
            errorCode,
            message
        };

        var json = JsonSerializer.Serialize(errorResponse);
        return context.Response.WriteAsync(json);
    }
}
