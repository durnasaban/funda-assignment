using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog.Web;
using System;

namespace ConsumerWorkerService
{
    using BackgroundServices;
    using Options;

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
                .UseWindowsService()
                .UseSystemd()
                .ConfigureServices((hostContext, services) =>
                {
                    var configuration = hostContext.Configuration;

                    services
                        .Configure<CachingTopLocationBasedObjectsOptions>(
                                configuration
                                    .GetSection(CachingTopLocationBasedObjectsOptions.CachingTopLocationBasedObjects))
                        .Configure<CachingTopLocationAndFeatureBasedObjectsOptions>(
                                configuration
                                    .GetSection(CachingTopLocationAndFeatureBasedObjectsOptions.CachingTopLocationAndFeatureBasedObjects))
                        .AddHostedService<CachingTopLocationBasedObjectsWorker>()
                        .AddHostedService<CachingTopLocationAndFeatureBasedObjectsWorker>();
                })
                .UseNLog();
    }
}
