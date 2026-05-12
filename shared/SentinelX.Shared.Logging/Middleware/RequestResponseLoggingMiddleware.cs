using Serilog;
using Serilog.Core;
using Serilog.Events;
using Microsoft.AspNetCore.Http;

namespace SentinelX.Shared.Logging.Middleware;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public RequestResponseLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
        _logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.WithProperty("Service", "SentinelX")
            .CreateLogger();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            _logger.Information("HTTP Request: {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            await _next(context);

            _logger.Information("HTTP Response: {StatusCode}",
                context.Response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An unhandled exception occurred");
            throw;
        }
    }
}
