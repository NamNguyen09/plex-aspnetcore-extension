using Microsoft.Extensions.Primitives;

namespace Plex.Security.Headers.AspNetCore.Middlewares;
public class CorrelationMiddleware
{
    private readonly RequestDelegate _next;

    public CorrelationMiddleware(RequestDelegate next)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
    }
    public async Task Invoke(HttpContext httpContext)
    {
        string? correlationId = null;
        if (httpContext.Request.Headers.TryGetValue(HeaderKeys.CorrelationId, out StringValues correlationIds))
        {
            correlationId = correlationIds.ToString();
        }
        else
        {
            correlationId = Convert.ToString(httpContext.Request.Query["correlationid"]);
        }

        if (string.IsNullOrEmpty(correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
        }

        httpContext.Request.Headers.Append(HeaderKeys.CorrelationId, correlationId);

        httpContext.Response.OnStarting(() =>
        {
            if (!httpContext.Response.Headers.TryGetValue(HeaderKeys.CorrelationId, out correlationIds))
            {
                httpContext.Response.Headers.Append(HeaderKeys.CorrelationId, correlationId);
            }
            return Task.CompletedTask;
        });
        await _next.Invoke(httpContext);
    }
}
