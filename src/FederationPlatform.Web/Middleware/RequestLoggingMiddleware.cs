using System.Diagnostics;

namespace FederationPlatform.Web.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            
            var statusCode = context.Response.StatusCode;
            var method = context.Request.Method;
            var path = context.Request.Path;
            var queryString = context.Request.QueryString;
            var elapsed = stopwatch.ElapsedMilliseconds;
            
            if (statusCode >= 400)
            {
                _logger.LogWarning(
                    "HTTP {Method} {Path}{QueryString} responded {StatusCode} in {Elapsed}ms",
                    method, path, queryString, statusCode, elapsed);
            }
            else
            {
                _logger.LogInformation(
                    "HTTP {Method} {Path}{QueryString} responded {StatusCode} in {Elapsed}ms",
                    method, path, queryString, statusCode, elapsed);
            }
        }
    }
}

public static class RequestLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLoggingMiddleware>();
    }
}
