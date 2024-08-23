using Domain.interfaces.services;

namespace worker
{
    public class Worker(ILogger<Worker> logger, IServiceScopeFactory services) : BackgroundService
    {
        private readonly ILogger<Worker> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private IServiceScopeFactory Services { get; } = services ?? throw new ArgumentNullException(nameof(services));

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker iniciando em: {time}", DateTimeOffset.Now);
                    using (var scope = Services.CreateScope())
                    {
                        var tarefaService = scope.ServiceProvider.GetRequiredService<ITarefaService>();
                        await tarefaService.CreateTarefa();
                    }
                }
            await Task.Delay(1000, stoppingToken);
        }
        _logger.LogInformation("Worker finalizado em: {time}", DateTimeOffset.Now);

        }
    }
}
