using System.Globalization;
using Microsoft.AspNetCore.HttpsPolicy;

namespace Plex.Security.Headers.AspNetCore.Extenstions;
internal static class HstsOptionsExtensions
{
    public static string BuildHeaderValue(this HstsOptions hstsOptions)
    {
        ArgumentNullException.ThrowIfNull(hstsOptions);

        var maxAge = Convert.ToInt64(Math.Floor(hstsOptions.MaxAge.TotalSeconds))
                    .ToString(CultureInfo.InvariantCulture);
        string headerValue = "max-age=" + maxAge;
        if (hstsOptions.IncludeSubDomains)
        {
            headerValue += "; includeSubDomains";
        }
        if (hstsOptions.Preload)
        {
            headerValue += "; preload";
        }
        return headerValue;
    }
}