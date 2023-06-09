using taxonomy_api.Health;
using taxonomy_api.HttpService;

namespace taxonomy_api.Repository
{
    public class IndexAssociationService : AbstractHostedService
    {
        private readonly ITaxonomyRepository _repository;
        private readonly FrostHttpService _frostService;
        private readonly ILogger<IndexAssociationService> _logger;
        private readonly StartupHealthCheck _healthCheck;

        public IndexAssociationService(FrostHttpService frostService, ITaxonomyRepository repository, ILogger<IndexAssociationService> logger, StartupHealthCheck healthCheck)
            : base("Association", logger, lifeInMinutes:16)
            => (_repository, _frostService, _logger, _healthCheck) = (repository, frostService, logger, healthCheck);


        protected override bool DoJob()
        {
            var indexId = TaxonomyIndex._indexId;
            _logger.LogInformation($"Updating associations {indexId}");
            try
            {
                var associations = _frostService.GetAssociations(indexId).Result?
                                                    .Associations
                                                    .Where(a => "exactMatchOf".Equals(a.AssociationType))
                                                    .GroupBy(a => a.Source.Identifier)
                                                    .ToDictionary(g => g.Key, g => g.Select(a => a.Destination.Identifier));
               if (associations != null) { 
                    _repository.SetLinks(associations);
                    _healthCheck.LinkRefreshFailure = false;
                    return true;
                }

                _logger.LogWarning("No associations received");
            }
            catch (Exception ex)
            {
                _logger.LogError("No associations received", ex);
            }
            return false;
        }

        protected override void OnUnhealthy()
        {
            _healthCheck.LinkRefreshFailure = true;
        }
    }
}

