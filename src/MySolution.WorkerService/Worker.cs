using Hangfire;

namespace MySolution.WorkerService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

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

    [AutomaticRetry(Attempts = 3)]
    public void PerformJob()
    {
        _logger.LogInformation("Performing scheduled job at: {time}", DateTimeOffset.Now);
        // Job logic here
    }
}
