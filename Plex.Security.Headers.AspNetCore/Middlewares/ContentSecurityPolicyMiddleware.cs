using System.Security.Cryptography;

namespace Plex.Security.Headers.AspNetCore.Middlewares;
public class ContentSecurityPolicyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _cspHeader;

    const string _xContentSecurityPolicyKey = "Content-Security-Policy";
    const string _xContentTypeOptionsKey = "X-Content-Type-Options";
    const string _xFrameOptionsKey = "X-Frame-Options";
    const string _referrerPolicyKey = "Referrer-Policy";   

    public ContentSecurityPolicyMiddleware(RequestDelegate next, string cspHeader)
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

        if (!ctx.Response.Headers.ContainsKey(_xContentSecurityPolicyKey)
                  && !string.IsNullOrWhiteSpace(_cspHeader))
        {
            string nonce = Convert.ToBase64String(GenerateRandomNonce(16));
            ctx.Items["nonce"] = nonce;
            ctx.Response.Headers.Append(_xContentSecurityPolicyKey, _cspHeader.Replace("{nonce}", nonce));
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
