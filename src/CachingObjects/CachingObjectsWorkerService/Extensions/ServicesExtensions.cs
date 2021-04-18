using Microsoft.Extensions.DependencyInjection;

namespace CachingObjectsWorkerService.Extensions
{
    using Data;
    using Repositories;
    using Services;

    public static class ServicesExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services) =>
            services
                .AddTransient<ITopAgentsCachingService, TopAgentsCachingService>()
                .AddTransient<IFundaContext, FundaContext>()
                .AddTransient<IStagingObjectRepository, StagingObjectRepository>()
                .AddTransient<ITopAgentsRepository, TopAgentsRepository>();
    }
}
