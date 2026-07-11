using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using StocksAssignment.Domain.Exceptions;

namespace StocksAssignment.Api.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = HttpStatusCode.InternalServerError;
            var message = "An internal server error occurred.";

            if (exception is ValidationException valEx)
            {
                statusCode = HttpStatusCode.BadRequest;
                message = valEx.Message;
                _logger.LogWarning("Input validation failed: {Message}", message);
            }
            else if (exception is ServiceUnavailableException svcEx)
            {
                statusCode = HttpStatusCode.ServiceUnavailable;
                message = svcEx.Message;
                _logger.LogError(exception, "Downstream service unavailable: {Message}", message);
            }
            else
            {
                _logger.LogCritical(exception, "An unhandled exception occurred in the API pipeline.");
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new { message = message };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
