using System.Globalization;
using Microsoft.Net.Http.Headers;

namespace Plex.Security.Headers.AspNetCore.Middlewares;
public class CacheControlMiddleware
{
    private readonly RequestDelegate _next;

    private readonly int _cacheMaxAge;

    public CacheControlMiddleware(RequestDelegate next, int cacheMaxAgeInDays)
    {
        _next = next;
        _cacheMaxAge = cacheMaxAgeInDays;
    }

    public async Task InvokeAsync(HttpContext ctx)
    {
        string path = ctx.Request.Path;
        if (!string.IsNullOrWhiteSpace(path) && (path.EndsWith(".js") || path.EndsWith(".png") || path.EndsWith(".mp3")
            || path.EndsWith(".ico") || path.EndsWith(".map") || path.EndsWith(".svg")))
        {
            ctx.Response.Headers.Remove(HeaderNames.CacheControl);
            ctx.Response.Headers.Remove(HeaderNames.Expires);

            ctx.Response.Headers.Append(HeaderNames.CacheControl, $"public, max-age={60 * 60 * 24 * _cacheMaxAge}");
            ctx.Response.Headers.Append(HeaderNames.Expires, DateTime.UtcNow.AddDays(_cacheMaxAge).ToString("R", CultureInfo.InvariantCulture));
        }

        if (ctx.Request.Method.Equals(HttpMethod.Get))
        {
            ctx.Response.GetTypedHeaders().CacheControl =
            new CacheControlHeaderValue()
            {
                Public = true,
                MaxAge = TimeSpan.FromMinutes(60)
            };
            ctx.Response.Headers[HeaderNames.Vary] = new string[] { "Accept-Encoding" };
        }

        await _next(ctx);
    }
}
