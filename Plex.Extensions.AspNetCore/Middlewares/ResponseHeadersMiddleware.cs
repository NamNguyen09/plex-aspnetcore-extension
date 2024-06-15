using System.Globalization;
using System.Security.Cryptography;
using Microsoft.Net.Http.Headers;

namespace Plex.Extensions.AspNetCore.Middlewares;
public class ResponseHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _cacheMaxAgeOneMonth = (60 * 60 * 24 * 365).ToString();
    private readonly string _cspHeader;

    const string _xContentSecurityPolicyKey = "Content-Security-Policy";
    const string _xContentTypeOptionsKey = "X-Content-Type-Options";
    const string _xFrameOptionsKey = "X-Frame-Options";
    const string _referrerPolicyKey = "Referrer-Policy";
    const string _serverKey = "Server";
    const string _xAspNetVersionKey = "X-AspNet-Version";
    const string _xPoweredByKey = "X-Powered-By";
    const string _xHtmlMinificationPoweredByKey = "X-HTML-Minification-Powered-By";

    public ResponseHeadersMiddleware(RequestDelegate next, 
                                     string cspHeader)
    {
        _next = next;
        _cspHeader = cspHeader;
    }

    public async Task InvokeAsync(HttpContext ctx)
    {
        if (!ctx.Response.Headers.ContainsKey(_xContentTypeOptionsKey))
        {
            ctx.Response.Headers.Append(_xContentTypeOptionsKey, "nosniff");
        }

        if (!ctx.Response.Headers.ContainsKey(_xFrameOptionsKey))
        {
            ctx.Response.Headers.Append(_xFrameOptionsKey, "SAMEORIGIN");
        }

        if (!ctx.Response.Headers.ContainsKey(_referrerPolicyKey))
        {
            ctx.Response.Headers.Append(_referrerPolicyKey, "strict-origin-when-cross-origin");
        }

        string path = ctx.Request.Path;
        if (!string.IsNullOrWhiteSpace(path) && (path.EndsWith(".js") || path.EndsWith(".png") || path.EndsWith(".mp3")
            || path.EndsWith(".ico") || path.EndsWith(".map") || path.EndsWith(".svg")))
        {
            ctx.Response.Headers.Remove(HeaderNames.CacheControl);
            ctx.Response.Headers.Remove(HeaderNames.Expires);

            ctx.Response.Headers.Append(HeaderNames.CacheControl, $"public, max-age={_cacheMaxAgeOneMonth}");
            ctx.Response.Headers.Append(HeaderNames.Expires, DateTime.UtcNow.AddDays(30).ToString("R", CultureInfo.InvariantCulture));
        }

        if (!ctx.Response.Headers.ContainsKey(_xContentSecurityPolicyKey)
                  && !string.IsNullOrWhiteSpace(_cspHeader))
        {
            string nonce = Convert.ToBase64String(GenerateRandomNonce(16));
            ctx.Items["nonce"] = nonce;
            ctx.Response.Headers.Append(_xContentSecurityPolicyKey, _cspHeader.Replace("{nonce}", nonce));
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

        if (ctx.Response.Headers.ContainsKey(_xAspNetVersionKey))
        {
            ctx.Response.Headers.Remove(_xAspNetVersionKey);
        }
        if (ctx.Response.Headers.ContainsKey(_xPoweredByKey))
        {
            ctx.Response.Headers.Remove(_xPoweredByKey);
        }

        ctx.Response.OnStarting(state =>
        {
            var httpContext = (HttpContext)state;
            httpContext.Response.Headers.Remove(_serverKey);
            ctx.Response.Headers.Remove(_xHtmlMinificationPoweredByKey);
            return Task.CompletedTask;
        }, ctx);

        await _next(ctx);
    }

    static byte[] GenerateRandomNonce(int length)
    {
        // Create a byte array to hold the random nonce
        byte[] nonce = new byte[length];

        using (var rng = RandomNumberGenerator.Create())
        {
            // Fill the nonce array with random data
            rng.GetBytes(nonce);
        }

        return nonce;
    }
}
