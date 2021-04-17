using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsumerWorkerService.BackgroundServices
{
    using Options;

    public class CachingTopLocationAndFeatureBasedObjectsWorker : BackgroundService
    {
        private readonly int _executionPeriodInMinutes;
        private readonly ILogger<CachingTopLocationAndFeatureBasedObjectsWorker> _logger;

        public CachingTopLocationAndFeatureBasedObjectsWorker(
            IOptions<CachingTopLocationBasedObjectsOptions> objectsOptions,
            ILogger<CachingTopLocationAndFeatureBasedObjectsWorker> logger)
        {
            _logger = logger;

            var options = objectsOptions ?? throw new ArgumentNullException(nameof(objectsOptions));

            _executionPeriodInMinutes = options.Value.ExecutionPeriodInMinutes;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                await Task.Delay(TimeSpan.FromMinutes(_executionPeriodInMinutes), stoppingToken);
            }
        }
    }
}
