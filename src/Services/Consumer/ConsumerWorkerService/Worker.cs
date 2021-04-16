using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsumerWorkerService
{
    using Options;

    public class Worker : BackgroundService
    {
        private readonly int _executionPeriodInMinutes;
        private readonly ILogger<Worker> _logger;

        public Worker(
            IOptions<SchedulingOptions> schedulingOptions,
            ILogger<Worker> logger)
        {
            _logger = logger;

            var options = schedulingOptions ?? throw new ArgumentNullException(nameof(schedulingOptions));

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
