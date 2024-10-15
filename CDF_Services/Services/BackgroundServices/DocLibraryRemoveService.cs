using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CDF_Services.Services.BackgroundServices
{
    public class DocLibraryRemoveService : IHostedService, IDisposable
    {
        private readonly ILogger<DocLibraryRemoveService> _logger;
        private Timer _timer;

        public DocLibraryRemoveService(ILogger<DocLibraryRemoveService> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            _logger.LogInformation("Timed Hosted Service is working.");
            // Your scheduled task logic here
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
