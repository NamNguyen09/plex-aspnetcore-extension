using System.Security.Cryptography;
using Microsoft.Net.Http.Headers;

namespace Plex.Security.Headers.AspNetCore.Middlewares;
public class ContentSecurityPolicyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _cspHeader;
    private readonly bool _isSpaApp;
    private readonly string? _nonceValue;

    public ContentSecurityPolicyMiddleware(RequestDelegate next,
                                           string cspHeader,
                                           string? nonceValue,
                                           bool isSpaApp)
    {
        _next = next;
        _cspHeader = cspHeader;
        _nonceValue = nonceValue;
        _isSpaApp = isSpaApp;
    }

    public async Task InvokeAsync(HttpContext ctx)
    {
        if (!ctx.Response.Headers.ContainsKey(HeaderNames.ContentSecurityPolicy)
            && !string.IsNullOrWhiteSpace(_cspHeader))
        {
            if (_isSpaApp)
            {
                if (HttpMethods.IsGet(ctx.Request.Method)
                 && !string.IsNullOrWhiteSpace(ctx.Request.Headers.Accept)
                 && ctx.Request.Headers.Accept.Contains("html"))
                {
                    AddCspToResponseHeader(ctx);
                }
            }
            else
            {
                AddCspToResponseHeader(ctx);
            }
        }

        await _next(ctx);
    }
    void AddCspToResponseHeader(HttpContext ctx)
    {
        string nonce = _nonceValue ?? Convert.ToBase64String(GenerateRandomNonce(16));
        ctx.Items["nonce"] = nonce;
        ctx.Response.Headers.Append(HeaderNames.ContentSecurityPolicy, _cspHeader.Replace("{nonce}", nonce));
        if (!_isSpaApp) return;
        ctx.Response.Cookies.Append("cxnonce", nonce, new CookieOptions
        {
            HttpOnly = false,
            Secure = true,
            SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax
        });
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
