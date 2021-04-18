using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CachingObjectsWorkerService.Extensions
{
    public static class RedisExtensions
    {
        public static IServiceCollection AddRedisConfig(this IServiceCollection services, IConfiguration configuration) => 
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetValue<string>("CacheSettings:ConnectionString");
            });
    }
}
