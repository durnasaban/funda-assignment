using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CachingObjectsWorkerService.Extensions
{
    using Options;

    public static class OptionsExtensions
    {
        public static IServiceCollection AddOptionsConfig(this IServiceCollection services, IConfiguration configuration) =>
            services
                .Configure<TopAgentsCachingOptions>(
                    configuration.GetSection(TopAgentsCachingOptions.TopAgentsCaching))
                .Configure<MongoDbSettingsOptions>(
                    configuration.GetSection(MongoDbSettingsOptions.MongoDbSettings));


    }
}
