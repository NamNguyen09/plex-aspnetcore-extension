using Microsoft.AspNetCore.HttpsPolicy;

namespace Plex.Extensions.AspNetCore;
public static class IServiceCollectionExtensions
{
    public static IServiceCollection RegisterHsts(this IServiceCollection services, 
                                                  Action<HstsOptions>? configureOptions = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (configureOptions == null)
        {
            List<string> removedExcludeHosts = ["localhost", "127.0.0.1", "[::1]"];
            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(60);
                foreach (string item in removedExcludeHosts)
                {
                    options.ExcludedHosts.Remove(item);
                }
            });

            return services;
        }

        services.AddHsts(configureOptions);

        return services;
    }
}
