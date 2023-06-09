using AutoMapper;
using API.Taxonomy.Domain.Model;
using taxonomy_api.Health;
using taxonomy_api.HttpService;

namespace taxonomy_api.Repository;

public class TaxonomyRepository: ITaxonomyRepository
{
    public const string filterKey = "filter";
    private readonly FrostHttpService _frostService;
    private readonly TaxonomyCache _taxonomyCache;
    private readonly IMapper _mapper;
    private readonly StartupHealthCheck _healthCheck;
    private IDictionary<Guid, IEnumerable<Guid>> associations = new Dictionary<Guid, IEnumerable<Guid>>();

    public TaxonomyRepository(FrostHttpService frostService, TaxonomyCache taxonomyCache, IMapper mapper, StartupHealthCheck healthCheck)
        => (_frostService, _taxonomyCache, _mapper, _healthCheck) = (frostService, taxonomyCache, mapper, healthCheck);

    public IEnumerable<Taxonomy> GetByAssociation(Guid id, IEnumerable<string>? nodeType = null)
    {
            return associations.Where(a => a.Key.Equals(id))
                               .SelectMany(a => a.Value)
                               .AsParallel()
                               .Select(GetOrFetch)
                               .Where(t => t != null)
                               .Select(t => FilterTaxonomy(t, nodeType))
                               .Where(t => t.Children.Count > 0)
                               .ToList();
    }

    public Taxonomy? GetById(Guid id, IEnumerable<string>? nodeType = null)
        => FilterTaxonomy(GetOrFetch(id), nodeType);
    

    public Taxonomy? GetIndex(IEnumerable<string>? nodeType = null)
        => FilterTaxonomy(_taxonomyCache.Get(TaxonomyIndex._indexId), nodeType);

    public Taxonomy? GetChildById(Guid documentId, Guid id, IEnumerable<string>? nodeType = null)
    {
        var response = _taxonomyCache.Get(id);
        if (response == null)
        {
            GetOrFetch(documentId);
            response = _taxonomyCache.Get(id);
        }
        return FilterTaxonomy(response, nodeType);
    }

    public void SetLinks(IDictionary<Guid, IEnumerable<Guid>> links)
    {
        if (links != null)
        {
            associations = links;
            _healthCheck.LinksCompleted = true;
        }
    }

    private Taxonomy? GetOrFetch(Guid id)
        => _healthCheck.StartupCompleted() ? _taxonomyCache.GetOrFetch(id, (i) => _frostService.GetTaxonomyByIdAsync(i).Result) : null;

    private Taxonomy FilterTaxonomy(Taxonomy? taxonomy, IEnumerable<string>? nodeType)
        => _mapper.Map<Taxonomy>(taxonomy, o => o.Items[filterKey] = nodeType);

}

