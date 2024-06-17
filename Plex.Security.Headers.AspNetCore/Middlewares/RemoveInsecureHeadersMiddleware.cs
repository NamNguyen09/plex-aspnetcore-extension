using Microsoft.Net.Http.Headers;

namespace Plex.Security.Headers.AspNetCore.Middlewares;
public class RemoveInsecureHeadersMiddleware
{
	private readonly RequestDelegate _next;

	public RemoveInsecureHeadersMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	public async Task Invoke(HttpContext context)
	{
		context.Response.OnStarting(state =>
		{
			var httpContext = (HttpContext)state;

			if (httpContext.Response.Headers.ContainsKey(HeaderNames.Server))
				httpContext.Response.Headers.Remove(HeaderNames.Server);

			if (httpContext.Response.Headers.ContainsKey(HeaderNames.XPoweredBy))
				httpContext.Response.Headers.Remove(HeaderNames.XPoweredBy);

			if (httpContext.Response.Headers.ContainsKey(HeaderKeys.XHtmlMinificationPoweredByKey))
				httpContext.Response.Headers.Remove(HeaderKeys.XHtmlMinificationPoweredByKey);

			if (httpContext.Response.Headers.ContainsKey(HeaderKeys.XAspNetVersionKey))
				httpContext.Response.Headers.Remove(HeaderKeys.XAspNetVersionKey);

			return Task.CompletedTask;
		}, context);

		await _next(context);
	}
}
