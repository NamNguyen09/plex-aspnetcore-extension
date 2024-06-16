using Microsoft.AspNetCore.HttpsPolicy;
using Plex.Security.Headers.AspNetCore.Middlewares;

namespace Plex.Security.Headers.AspNetCore.Extenstions;
public static class AppBuilderExtensions
{
    public static IApplicationBuilder UseStrictTransportSecurity(
                                        this IApplicationBuilder app,
                                        HstsOptions hstsOptions)
    {
        return app.UseMiddleware<StrictTransportSecurityMiddleware>(hstsOptions);
    }
    public static IApplicationBuilder UseStrictTransportSecurity(this IApplicationBuilder app)
    {
        return app.UseMiddleware<StrictTransportSecurityMiddleware>();
    }
    public static IApplicationBuilder UseCps(this IApplicationBuilder app,
                                            string cpsHeader)
    {
        return app.UseMiddleware<ContentSecurityPolicyMiddleware>(cpsHeader);
    }
    public static IApplicationBuilder UseRemoveInsecureHeaders(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RemoveInsecureHeadersMiddleware>();
    }
    public static IApplicationBuilder UseCacheControl(this IApplicationBuilder app, 
                                                      int cacheMaxAgeInDays = 30)
    {
        return app.UseMiddleware<CacheControlMiddleware>(cacheMaxAgeInDays);
    }
}
