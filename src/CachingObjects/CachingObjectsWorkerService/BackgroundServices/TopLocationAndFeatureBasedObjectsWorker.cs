using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CachingObjectsWorkerService.BackgroundServices
{
    using Options;

    public class TopLocationAndFeatureBasedObjectsWorker : BackgroundService
    {
        private readonly int _executionPeriodInMinutes;
        private readonly ILogger<TopLocationAndFeatureBasedObjectsWorker> _logger;

        public TopLocationAndFeatureBasedObjectsWorker(
            IOptions<TopLocationBasedObjectsOptions> objectsOptions,
            ILogger<TopLocationAndFeatureBasedObjectsWorker> logger)
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
