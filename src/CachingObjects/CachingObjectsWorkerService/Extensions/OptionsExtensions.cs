using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CachingObjectsWorkerService.Extensions
{
    using Options;

    public static class OptionsExtensions
    {
        public static IServiceCollection AddOptionsConfig(this IServiceCollection services, IConfiguration configuration) =>
            services
                .Configure<TopLocationBasedObjectsOptions>(
                    configuration.GetSection(TopLocationBasedObjectsOptions.TopLocationBasedObjects))
                .Configure<TopLocationAndFeatureBasedObjectsOptions>(
                    configuration.GetSection(TopLocationAndFeatureBasedObjectsOptions.TopLocationAndFeatureBasedObjects));


    }
}
