using Microsoft.Net.Http.Headers;

namespace Plex.Security.Headers.AspNetCore.Extenstions;
internal static class HttpRequestExtensions
{
	internal static bool IsStaticFile(this HttpRequest request)
	{
		if (string.IsNullOrWhiteSpace(request.Path)) return false;
		string path = request.Path;
		return path.EndsWith(".js")
				|| path.EndsWith(".png")
				|| path.EndsWith(".mp3")
				|| path.EndsWith(".ico")
				|| path.EndsWith(".map")
				|| path.EndsWith(".svg")
				|| path.EndsWith(".ttf")
				|| path.EndsWith(".woff")
				|| path.EndsWith(".woff2");
	}
	internal static bool HasCacheControlHeader(this HttpContext context)
	{
		return context != null && context.Response.Headers.ContainsKey(HeaderNames.CacheControl);
	}
}
