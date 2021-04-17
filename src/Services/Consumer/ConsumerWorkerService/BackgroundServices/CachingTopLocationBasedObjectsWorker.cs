using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsumerWorkerService.BackgroundServices
{
    using Options;

    public class CachingTopLocationBasedObjectsWorker : BackgroundService
    {
        private readonly int _executionPeriodInMinutes;
        private readonly ILogger<CachingTopLocationBasedObjectsWorker> _logger;

        public CachingTopLocationBasedObjectsWorker(
            IOptions<CachingTopLocationBasedObjectsOptions> objectsOptions,
            ILogger<CachingTopLocationBasedObjectsWorker> logger)
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
