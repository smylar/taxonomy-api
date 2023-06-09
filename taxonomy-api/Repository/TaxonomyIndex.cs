using System;
using taxonomy_api.Health;
using taxonomy_api.HttpService;

namespace taxonomy_api.Repository;

public class TaxonomyIndex : AbstractHostedService
{
    public static readonly Guid _indexId;
    private readonly TaxonomyCache _cache;
    private readonly FrostHttpService _frostService;
    private readonly ILogger<TaxonomyIndex> _logger;
    private readonly StartupHealthCheck _healthCheck;

    static TaxonomyIndex()
    {
        var indexId = Environment.GetEnvironmentVariable("INDEX_ID");
        _indexId = indexId == null ? Guid.Empty : Guid.Parse(indexId);
    }

    public TaxonomyIndex(TaxonomyCache cache, FrostHttpService frostService, ILogger<TaxonomyIndex> logger, StartupHealthCheck healthCheck)
        : base("Index", logger, lifeInMinutes:15)
        => (_cache, _frostService, _logger, _healthCheck) = (cache, frostService, logger, healthCheck);

    protected override bool DoJob()
    {
        _logger.LogInformation($"Updating index {_indexId}");
        try
        {
            var newIndex = _frostService.GetTaxonomyByIdAsync(_indexId).Result;
            if (newIndex != null)
            {
                _cache.SetIndexEntries(newIndex);
                _healthCheck.IndexCompleted = true;
                _healthCheck.IndexRefreshFailure = false;
                return true;
            }

            _logger.LogWarning("No index received");
        }
        catch (Exception ex)
        {
            _logger.LogError("No index received", ex);
        }

        return false;
    }

    protected override void OnUnhealthy()
    {
        _healthCheck.IndexRefreshFailure = true;
    }
}

