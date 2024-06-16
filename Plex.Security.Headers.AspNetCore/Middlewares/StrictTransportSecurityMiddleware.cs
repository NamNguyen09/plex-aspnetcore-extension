using Microsoft.AspNetCore.HttpsPolicy;
using Plex.Security.Headers.AspNetCore.Extenstions;

namespace Plex.Security.Headers.AspNetCore.Middlewares;
public class StrictTransportSecurityMiddleware
{
    private const string _headerName = "Strict-Transport-Security";
    private readonly RequestDelegate _next;
    private readonly string _headerValue;

    public StrictTransportSecurityMiddleware(RequestDelegate next,
                                             HstsOptions? options = null)
    {
        _next = next;
        options ??= new();
        _headerValue = options.BuildHeaderValue();
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.IsHttps && !ContainsHstsHeader(context.Response))
        {
            context.Response.Headers.Append(_headerName, _headerValue);
        }
        await _next(context);
    }

    bool ContainsHstsHeader(HttpResponse response)
    {
        return response.Headers.Any(h => h.Key.Equals(_headerName, StringComparison.OrdinalIgnoreCase));
    }
}