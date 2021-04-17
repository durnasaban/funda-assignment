using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CachingObjectsWorkerService.BackgroundServices
{
    using Services;
    using Options;

    public class TopAgentsWorker : BackgroundService
    {
        private readonly ITopAgentsCachingService _service;
        private readonly int _executionPeriodInMinutes;
        private readonly ILogger<TopAgentsWorker> _logger;

        public TopAgentsWorker(
            ITopAgentsCachingService service,
            IOptions<TopAgentsCachingOptions> objectsOptions,
            ILogger<TopAgentsWorker> logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger;

            var options = objectsOptions ?? throw new ArgumentNullException(nameof(objectsOptions));

            _executionPeriodInMinutes = options.Value.ExecutionPeriodInMinutes;

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await ExecuteAsync();

                await Task.Delay(TimeSpan.FromMinutes(_executionPeriodInMinutes), stoppingToken);
            }
        }

        private async Task ExecuteAsync()
        {
            try
            {
                _logger.LogInformation("Top Location Based Objects Worker started running at: {time}", DateTimeOffset.Now);

                await _service.ProsessCachingObjectsAsync();

                _logger.LogInformation("Top Location Based Objects Worker finished successfully at: {time}", DateTimeOffset.Now);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occured while executing Top Location Based Objects Worker");
                _logger.LogWarning("Top Location Based Objects Worker finished with exception at: {time}", DateTimeOffset.Now);
            }
        }
    }
}
