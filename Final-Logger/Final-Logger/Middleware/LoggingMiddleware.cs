using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IAppLogger _logger;

    public LoggingMiddleware(RequestDelegate next, IAppLogger logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
      
        await _logger.LogAsync(LogLevel.Information, $"Handling request: {context.Request.Method} {context.Request.Path}");

        await _next(context);

       
        await _logger.LogAsync(LogLevel.Information, $"Finished handling request: {context.Response.StatusCode}");
    }
}