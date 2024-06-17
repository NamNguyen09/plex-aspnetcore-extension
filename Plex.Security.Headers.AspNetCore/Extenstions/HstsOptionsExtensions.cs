using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.HttpsPolicy;

namespace Plex.Security.Headers.AspNetCore.Extenstions;
internal static class HstsOptionsExtensions
{
	const string _includeSubDomains = "; includeSubDomains";
	const string _preload = "; preload";
	public static string BuildHeaderValue(this HstsOptions hstsOptions)
	{
		ArgumentNullException.ThrowIfNull(hstsOptions);

		var maxAge = Convert.ToInt64(Math.Floor(hstsOptions.MaxAge.TotalSeconds)).ToString(CultureInfo.InvariantCulture);
		StringBuilder headerValue = new();
		headerValue.Append($"max-age={maxAge}");
		if (hstsOptions.IncludeSubDomains)
		{
			headerValue.Append(_includeSubDomains);
		}
		if (hstsOptions.Preload)
		{
			headerValue.Append(_preload);
		}

		return headerValue.ToString();
	}
}