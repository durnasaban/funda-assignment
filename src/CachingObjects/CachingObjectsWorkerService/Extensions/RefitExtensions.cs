using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using System;

namespace CachingObjectsWorkerService.Extensions
{
    using CachingObjectsWorkerService.Options;
    using ExternalServices;

    public static class RefitExtensions
    {
        public static IServiceCollection AddRefitConfig(this IServiceCollection services, IConfiguration configuration)
        {
            var options = configuration.GetSection(FundaApiOptions.FundaApi).Get<FundaApiOptions>();

            var uri = new Uri(options.BaseUrl);
            uri = new Uri(uri, options.Query);
            uri = new Uri(uri, options.Key);

            services
                .AddRefitClient<IFundaApi>(new RefitSettings
                {
                    ContentSerializer = new NewtonsoftJsonContentSerializer()
                })
                .ConfigureHttpClient(c => c.BaseAddress = uri);

            return services;
        }
    }
}
