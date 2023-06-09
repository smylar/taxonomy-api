namespace taxonomy_api.Repository;

public abstract class AbstractHostedService : IHostedService
{
    private readonly string _name;
    private readonly ILogger<AbstractHostedService> _logger;
    private readonly int _lifeInSeconds;
    private readonly int _attemptsBeforeUnhealthy;
    private readonly Random rand = new();
    private int refreshAttempts = 0;
    private Timer? _t;

    public AbstractHostedService(string name, ILogger<AbstractHostedService> logger, int lifeInMinutes, int attemptsBeforeUnhealthy = 3)
        => (_name, _logger, _lifeInSeconds, _attemptsBeforeUnhealthy) = (name, logger, lifeInMinutes*60, attemptsBeforeUnhealthy);

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (!cancellationToken.IsCancellationRequested)
        {
            _t = new Timer(c => RunJob(cancellationToken), null, 0, Timeout.Infinite);
        }
        else
        {
            _logger.LogWarning($"{_name} refreshing cancelled");
        }
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{_name} job stopped");
        _t?.Dispose();
        return Task.CompletedTask;
    }

    private void RunJob(CancellationToken cancellationToken)
    {
        var lifeInSeconds = GetInterval();
        try
        {
            if(DoJob())
            {
                lifeInSeconds = _lifeInSeconds;
                refreshAttempts = 0;
            }

            if (refreshAttempts >= _attemptsBeforeUnhealthy)
            {
                OnUnhealthy();
            }
        }
        finally
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                _t?.Change((int)TimeSpan.FromSeconds(lifeInSeconds).TotalMilliseconds, Timeout.Infinite);
            }
            else
            {
                _logger.LogWarning($"{_name} refresh cancelled");
            }
        }
    }

    private int GetInterval()
    {
        refreshAttempts++;
        int lifeInSeconds = (int)(1.2 * refreshAttempts * rand.Next(45, 75));
        if (lifeInSeconds > _lifeInSeconds)
        {
            lifeInSeconds = _lifeInSeconds;
        }

        return lifeInSeconds;
    }

    protected abstract bool DoJob();

    protected abstract void OnUnhealthy();
}

