using System.Diagnostics;

namespace worker
{
    public class Worker(ILogger<Worker> logger, ActivitySource activitySource) : BackgroundService
    {
        private readonly ILogger<Worker> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly ActivitySource _activitySource = activitySource ?? throw new ArgumentNullException(nameof(activitySource));

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
