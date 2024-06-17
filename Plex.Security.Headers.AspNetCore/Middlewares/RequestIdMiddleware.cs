using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;

namespace Plex.Security.Headers.AspNetCore.Middlewares;
public class RequestIdMiddleware
{

    private readonly RequestDelegate _next;

    public RequestIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public async Task Invoke(HttpContext httpContext)
    {
        string? requestId = null;
        if (httpContext.Request.Headers.TryGetValue(HeaderKeys.RequestId, out StringValues requestIds))
        {
            requestId = requestIds.FirstOrDefault(k => k.Equals(HeaderKeys.RequestId));
        }
        else
        {
            var requestIdFeature = httpContext.Features.Get<IHttpRequestIdentifierFeature>();
            if (requestIdFeature?.TraceIdentifier != null)
            {
                requestId = requestIdFeature.TraceIdentifier;
                httpContext.Request.Headers.Append(HeaderKeys.RequestId, requestId);
            }
        }

        httpContext.Response.OnStarting(() =>
        {
            if (!httpContext.Response.Headers.TryGetValue(HeaderKeys.RequestId, out requestIds))
            {
                httpContext.Response.Headers.Append(HeaderKeys.RequestId, requestId);
            }

            return Task.CompletedTask;
        });
        await _next.Invoke(httpContext);
    }
}
