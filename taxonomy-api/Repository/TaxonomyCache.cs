using System;
using System.Collections.Immutable;
using System.Reflection;
using API.Taxonomy.Domain.Model;
using Microsoft.Extensions.Caching.Memory;

namespace taxonomy_api.Repository;

public class TaxonomyCache
{
    private readonly MemoryCacheEntryOptions _indexOptions = new MemoryCacheEntryOptions() {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1),
        Priority = CacheItemPriority.NeverRemove
    };
    private readonly MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

    public Taxonomy? GetOrFetch(Guid guid, Func<Guid,Taxonomy?> getter)
    {
        Taxonomy? response = null;
        if (!_cache.TryGetValue(guid, out response)) {
            var newValue = getter.Invoke(guid);
            //we are using this for non index objects and a lot less likely to have simultaneous requests for the same resource
            //and less possible layers of children
            //so have not added any locking here .. yet
            if (newValue != null)
            {
                SetCacheEntries(newValue, t => _cache.Set(t.Id, t, TimeSpan.FromMinutes(10)));
                response = newValue; 
            }
        }
        return response;
    }

    public Taxonomy? Get(Guid guid)
    {
        return _cache.Get<Taxonomy?>(guid);
    }

    public void SetIndexEntries(Taxonomy index)
    {
        SetCacheEntries(index, t => _cache.Set(t.Id, t, _indexOptions));
    }

    private void SetCacheEntries(Taxonomy taxonomy, Func<Taxonomy, Taxonomy> cacheSetter)
    {
        cacheSetter.Invoke(taxonomy);

        IEnumerable<Taxonomy> currentTaxonomies = taxonomy.Children ?? ImmutableList<Taxonomy>.Empty;
        var newIndexMap = new Dictionary<Guid, Taxonomy>();
        while (currentTaxonomies.Count() > 0)
        {
            foreach (Taxonomy child in currentTaxonomies)
            {
                cacheSetter.Invoke(child);
            }
            currentTaxonomies = currentTaxonomies.SelectMany(t => t.Children);
        }
    }
}

