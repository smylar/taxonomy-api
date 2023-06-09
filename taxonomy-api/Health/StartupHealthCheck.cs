using System;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace taxonomy_api.Health;

public class StartupHealthCheck : IHealthCheck
{
    public bool IndexCompleted { get; set; } = false;
    public bool LinksCompleted { get; set; } = false;
    public bool IndexRefreshFailure { get; set; } = false;
    public bool LinkRefreshFailure { get; set; } = false;

    public bool StartupCompleted()
    {
        return IndexCompleted && LinksCompleted;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (IndexRefreshFailure)
        {
            return Task.FromResult(HealthCheckResult.Healthy("Unable to refresh index"));
        }

        if (LinkRefreshFailure)
        {
            return Task.FromResult(HealthCheckResult.Healthy("Unable to refresh associations"));
        }

        return IndexCompleted && LinksCompleted ? Task.FromResult(HealthCheckResult.Healthy("Startup complete"))
                                                : Task.FromResult(HealthCheckResult.Unhealthy("Startup in progress"));
    }

}

