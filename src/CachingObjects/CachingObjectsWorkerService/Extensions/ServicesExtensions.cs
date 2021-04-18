using Microsoft.Extensions.DependencyInjection;

namespace CachingObjectsWorkerService.Extensions
{
    using Services;

    public static class ServicesExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services) =>
            services                
                .AddTransient<ITopAgentsCachingService, TopAgentsCachingService>();        
    }
}
