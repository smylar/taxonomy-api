using System;
using System.Text.Json.Serialization;

namespace API.Taxonomy.Domain.Model;

public class FrostAssociationResponse
{
    [JsonPropertyName("CFAssociations")]
    public IEnumerable<Association> Associations { get; init; } = new List<Association>();
}

public class Association
{
    public Guid Identifier { get; init; }
    public string AssociationType { get; init; } = string.Empty;

    [JsonPropertyName("originNodeURI")]
    public AssociationLink Source { get; init; } = new AssociationLink();

    [JsonPropertyName("destinationNodeURI")]
    public AssociationLink Destination { get; init; } = new AssociationLink();
}

public class AssociationLink
{
    public Guid Identifier { get; init; }
    public string Title { get; init; } = string.Empty;
}