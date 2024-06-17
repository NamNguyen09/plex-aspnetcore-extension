using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Net.Http.Headers;
using Plex.Security.Headers.AspNetCore.Extenstions;

namespace Plex.Security.Headers.AspNetCore.Middlewares;
public class StrictTransportSecurityMiddleware
{
	private readonly RequestDelegate _next;
	private readonly string _headerValue;
	private readonly IList<string> _excludedHosts = [];

	public StrictTransportSecurityMiddleware(RequestDelegate next,
											 HstsOptions? hstsOptions = null)
	{
		_next = next;
		if (hstsOptions != null)
		{
			_excludedHosts = hstsOptions.ExcludedHosts;
		}

		hstsOptions ??= new();
		_headerValue = hstsOptions.BuildHeaderValue();
	}

	public async Task Invoke(HttpContext context)
	{
		if (!IsHostExcluded(context.Request.Host.Host)
			&& !ContainsHstsHeader(context.Response))
		{
			context.Response.Headers.Append(HeaderNames.StrictTransportSecurity, _headerValue);
		}

		await _next(context);
	}

	static bool ContainsHstsHeader(HttpResponse response)
	{
		return response.Headers.Any(h => h.Key.Equals(HeaderNames.StrictTransportSecurity, StringComparison.CurrentCultureIgnoreCase));
	}
	bool IsHostExcluded(string host)
	{
		for (var i = 0; i < _excludedHosts.Count; i++)
		{
			if (string.Equals(host, _excludedHosts[i], StringComparison.CurrentCultureIgnoreCase))
			{
				return true;
			}
		}

		return false;
	}
}