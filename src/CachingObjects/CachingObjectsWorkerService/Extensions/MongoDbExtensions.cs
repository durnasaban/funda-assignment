using Microsoft.Extensions.DependencyInjection;

namespace CachingObjectsWorkerService.Extensions
{
    using Repositories;
    using Data;

    public static class MongoDbExtensions
    {
        public static IServiceCollection AddMongoDb(this IServiceCollection services) =>
            services
                .AddTransient<IFundaContext, FundaContext>()
                .AddTransient<IStagingObjectRepository, StagingObjectRepository>();
    }   
}
