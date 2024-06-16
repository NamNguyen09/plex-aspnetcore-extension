using Microsoft.AspNetCore.HttpsPolicy;

namespace Plex.Security.Headers.AspNetCore.Extenstions;
internal static class HstsOptionsExtensions
{
    public static string BuildHeaderValue(this HstsOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        string headerValue = "max-age=" + options.MaxAge;
        if (options.IncludeSubDomains)
        {
            headerValue += "; includeSubDomains";
        }
        if (options.Preload)
        {
            headerValue += "; preload";
        }
        return headerValue;
    }
}