using System;
using API.Taxonomy.Domain.Model;

namespace taxonomy_api.Repository;

public interface ITaxonomyRepository
{
    Taxonomy? GetById(Guid id, IEnumerable<string>? nodeType = null);
    Taxonomy? GetIndex(IEnumerable<string>? nodeType);
    IEnumerable<Taxonomy> GetByAssociation(Guid id, IEnumerable<string>? nodeType);
    Taxonomy? GetChildById(Guid documentId, Guid id, IEnumerable<string>? nodeType);
    void SetLinks(IDictionary<Guid, IEnumerable<Guid>> links);
}

