using Microsoft.Extensions.DependencyInjection;

namespace CachingObjectsWorkerService.Extensions
{
    using BackgroundServices;

    public static class HostedServicesExtensions
    {
        public static IServiceCollection AddHostedServices(this IServiceCollection services) =>
            services
                .AddHostedService<TopAgentsWorker>();
    }
}
