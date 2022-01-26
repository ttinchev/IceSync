using System;
using System.Threading;
using System.Threading.Tasks;

using IceSync.Infrastructure.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

#pragma warning disable S3881

namespace IceSync.Business.Services
{
    /// <summary>The default implementation of <see cref="IHostedService"/> contract.</summary>
    public class TimedHostedService : IHostedService, IDisposable
    {
        private readonly ILogger<TimedHostedService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private Timer _timer;
        private int executionCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimedHostedService"/> class.
        /// </summary>
        /// <param name="logger">Adds looger.</param>
        /// <param name="scopeFactory">Add scoped services.</param>
        public TimedHostedService(ILogger<TimedHostedService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        /// <inheritdoc/>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _timer?.Dispose();
            GC.SuppressFinalize(this);
        }

        private void DoWork(object state)
        {
            var count = Interlocked.Increment(ref executionCount);

            using (var scope = _scopeFactory.CreateScope())
            {
                var workflowService = scope.ServiceProvider.GetRequiredService<IWorkflowService>();
                workflowService.SyncWorkflowsAsync();
            }

            _logger.LogInformation(
                "Timed Hosted Service is working. Count: {Count}", count);
        }
    }
}
