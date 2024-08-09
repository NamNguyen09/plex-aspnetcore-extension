namespace Plex.Security.Headers.AspNetCore.Middlewares;
public class NoHtmlCacheControlMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public NoHtmlCacheControlMiddleware(ILogger<NoHtmlCacheControlMiddleware> logger, 
                                        RequestDelegate next)
    {
        _logger = logger;
        _next = next;
    }

    public async Task Invoke(HttpContext ctx)
    {
        // Set Cache-Control header only once for HTML file
        if (HttpMethods.IsGet(ctx.Request.Method) 
            && !string.IsNullOrWhiteSpace(ctx.Request.Headers.Accept)
            && ctx.Request.Headers.Accept.Contains("html"))
        {
            AddNoCacheToResponseHeader(ctx);
        }

        await _next(ctx);
    }

    void AddNoCacheToResponseHeader(HttpContext context)
    {
        try
        {
            context.Response.Headers.Append(Microsoft.Net.Http.Headers.HeaderNames.CacheControl, "max-age=0, no-cache, must-revalidate");
            context.Response.Headers.Append(Microsoft.Net.Http.Headers.HeaderNames.Pragma, "no-cache");
            context.Response.Headers.Append(Microsoft.Net.Http.Headers.HeaderNames.Expires, "0");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error when add Cache-Control header to response");
        }
    }
}
