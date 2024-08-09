using System.Globalization;
using Microsoft.Net.Http.Headers;
using Plex.Security.Headers.AspNetCore.Extenstions;

namespace Plex.Security.Headers.AspNetCore.Middlewares;
public class CacheControlMiddleware
{
    private readonly RequestDelegate _next;

    private readonly TimeSpan _cacheMaxAge;
    private readonly TimeSpan _cacheMaxAgeStaticFiles;
    private readonly bool _cacheHttpGetMethods;

    public CacheControlMiddleware(RequestDelegate next,
                                  TimeSpan cacheMaxAge,
                                  TimeSpan cacheMaxAgeStaticFiles,
                                  bool cacheHttpGetMethods)
    {
        _next = next;
        _cacheMaxAge = cacheMaxAge;
        _cacheMaxAgeStaticFiles = cacheMaxAgeStaticFiles;
        _cacheHttpGetMethods = cacheHttpGetMethods;
    }

    public async Task InvokeAsync(HttpContext ctx)
    {
        if (!ctx.HasCacheControlHeader()
            && ctx.Request.IsStaticFile())
        {
            var maxAge = Convert.ToInt64(Math.Floor(_cacheMaxAgeStaticFiles.TotalSeconds)).ToString(CultureInfo.InvariantCulture);
            ctx.Response.Headers.Append(HeaderNames.CacheControl, $"max-age={maxAge}, immutable");
            ctx.Response.Headers.Append(HeaderNames.Expires, DateTime.UtcNow.AddDays(_cacheMaxAge.TotalDays).ToString("R", CultureInfo.InvariantCulture));
        }
        else if (_cacheHttpGetMethods
                && HttpMethods.IsGet(ctx.Request.Method)
                && !ctx.HasCacheControlHeader())
        {
            ctx.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue()
            {
                Public = true,
                MaxAge = _cacheMaxAge
            };
        }

        await _next(ctx);
    }
}
