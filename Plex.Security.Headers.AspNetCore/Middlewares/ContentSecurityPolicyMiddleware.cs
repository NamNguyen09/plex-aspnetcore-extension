using System.Security.Cryptography;
using Microsoft.Net.Http.Headers;

namespace Plex.Security.Headers.AspNetCore.Middlewares;
public class ContentSecurityPolicyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _cspHeader;

    public ContentSecurityPolicyMiddleware(RequestDelegate next, string cspHeader)
    {
        _next = next;
        _cspHeader = cspHeader;
    }

    public async Task InvokeAsync(HttpContext ctx)
    {
        if (!ctx.Response.Headers.ContainsKey(HeaderNames.ContentSecurityPolicy)
                  && !string.IsNullOrWhiteSpace(_cspHeader))
        {
            string nonce = Convert.ToBase64String(GenerateRandomNonce(16));
            ctx.Items["nonce"] = nonce;
            ctx.Response.Headers.Append(HeaderNames.ContentSecurityPolicy, _cspHeader.Replace("{nonce}", nonce));
        }

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
