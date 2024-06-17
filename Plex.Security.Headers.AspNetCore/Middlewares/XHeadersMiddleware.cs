using Microsoft.Net.Http.Headers;

namespace Plex.Security.Headers.AspNetCore.Middlewares;
public class XHeadersMiddleware
{
	private readonly RequestDelegate _next;

	public XHeadersMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	public async Task InvokeAsync(HttpContext ctx)
	{
		if (!ctx.Response.Headers.ContainsKey(HeaderNames.XContentTypeOptions))
		{
			ctx.Response.Headers.Append(HeaderNames.XContentTypeOptions, "nosniff");
		}

		if (!ctx.Response.Headers.ContainsKey(HeaderNames.XFrameOptions))
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
