using Microsoft.Extensions.Hosting;
using NLog.Web;
using System;

namespace CachingObjectsWorkerService
{
    using Extensions;
    using Microsoft.Extensions.Configuration;

    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

            try
            {
                logger.Debug("init main");

                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, builder) =>
                {
                    if (hostContext.HostingEnvironment.IsDevelopment())
                    {
                        builder.AddUserSecrets<Program>();
                    }
                })
                .UseWindowsService()
                .UseSystemd()
                .ConfigureServices((hostContext, services) =>
                {
                    var configuration = hostContext.Configuration;

                    services
                        .AddOptionsConfig(configuration)
                        .AddHostedServices()
                        .AddRefitConfig(configuration)
                        .AddRedisConfig(configuration)
                        .AddServices();
                })
                .UseNLog();
    }
}
