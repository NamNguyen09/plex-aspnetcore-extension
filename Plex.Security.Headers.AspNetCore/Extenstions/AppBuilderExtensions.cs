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
                                            string cpsHeader,
                                            string? nonceValue = null,
                                            bool isSpaApp = false)
    {
        return app.UseMiddleware<ContentSecurityPolicyMiddleware>(cpsHeader, nonceValue, isSpaApp);
    }
    public static IApplicationBuilder UseRemoveInsecureHeaders(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RemoveInsecureHeadersMiddleware>();
    }
    public static IApplicationBuilder UseCacheControl(this IApplicationBuilder app,
                                                      TimeSpan? cacheMaxAge = null,
                                                      TimeSpan? cacheMaxAgeStaticFiles = null,
                                                      bool cacheHttpGetMethods = false)
    {
        if (cacheMaxAge == null) cacheMaxAge = TimeSpan.FromMinutes(60);
        if (cacheMaxAgeStaticFiles == null) cacheMaxAgeStaticFiles = TimeSpan.FromDays(365);
        return app.UseMiddleware<CacheControlMiddleware>(cacheMaxAge, cacheMaxAgeStaticFiles, cacheHttpGetMethods);
    }
    public static IApplicationBuilder UseXHeaders(this IApplicationBuilder app, bool addXFrameOptions = true)
    {
        return app.UseMiddleware<XHeadersMiddleware>(addXFrameOptions);
    }
    public static IApplicationBuilder UseNoHtmlCacheControl(this IApplicationBuilder app)
    {
        return app.UseMiddleware<NoHtmlCacheControlMiddleware>();
    }
    public static IApplicationBuilder UseListEndpoints(this IApplicationBuilder app, EndpointDataSource endpointDataSource)
    {
        return app.UseMiddleware<ListEndpointsMiddleware>(endpointDataSource);
    }
    public static IApplicationBuilder UseCspMeta(this IApplicationBuilder app,
                                                 string cpsHeader,
                                                 string nonceValue)
    {
        return app.UseMiddleware<ContentSecurityPolicyMetaMiddleware>(cpsHeader, nonceValue);
    }
}
