using Microsoft.Net.Http.Headers;

namespace Plex.Security.Headers.AspNetCore.Middlewares;
public class XHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly bool _addXFrameOptions;

    public XHeadersMiddleware(RequestDelegate next,
                              bool addXFrameOptions)
    {
        _next = next;
        _addXFrameOptions = addXFrameOptions;
    }

    public async Task InvokeAsync(HttpContext ctx)
    {
        if (!ctx.Response.Headers.ContainsKey(HeaderNames.XContentTypeOptions))
        {
            ctx.Response.Headers.Append(HeaderNames.XContentTypeOptions, "nosniff");
        }

        if (_addXFrameOptions && !ctx.Response.Headers.ContainsKey(HeaderNames.XFrameOptions))
        {
            ctx.Response.Headers.Append(HeaderNames.XFrameOptions, "SAMEORIGIN");
        }

        if (!ctx.Response.Headers.ContainsKey(HeaderKeys.ReferrerPolicyKey))
        {
            ctx.Response.Headers.Append(HeaderKeys.ReferrerPolicyKey, "strict-origin-when-cross-origin");
        }

        await _next(ctx);
    }
}
