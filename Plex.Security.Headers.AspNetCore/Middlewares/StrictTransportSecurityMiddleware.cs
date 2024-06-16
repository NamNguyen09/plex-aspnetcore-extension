using Microsoft.AspNetCore.HttpsPolicy;
using Plex.Security.Headers.AspNetCore.Extenstions;

namespace Plex.Security.Headers.AspNetCore.Middlewares;
public class StrictTransportSecurityMiddleware
{
    private const string _headerName = "Strict-Transport-Security";
    private readonly RequestDelegate _next;
    private readonly string _headerValue;

    public StrictTransportSecurityMiddleware(RequestDelegate next,
                                             HstsOptions? hstsOptions = null)
    {
        _next = next;
        hstsOptions ??= new();
        _headerValue = hstsOptions.BuildHeaderValue();
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.IsHttps && !ContainsHstsHeader(context.Response))
        {
            context.Response.Headers.Append(_headerName, _headerValue);
        }
        await _next(context);
    }

    static bool ContainsHstsHeader(HttpResponse response)
    {
        return response.Headers.Any(h => h.Key.Equals(_headerName, StringComparison.OrdinalIgnoreCase));
    }
}