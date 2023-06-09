using System;
using System.Collections.Immutable;
using System.Text.Json.Serialization;
using API.Taxonomy.Domain.Converter;

namespace API.Taxonomy.Domain.Model;

public class Taxonomy
{
    [JsonConverter(typeof(GuidConverter))]
    public Guid Id { get; init; }

    public string? Title { get; init; }

    [JsonPropertyName("human_coding_scheme")]
    public string? HumanCodingScheme { get; init; }

    [JsonPropertyName("list_enumeration")]
    public string? ListEnumeration { get; init; }

    [JsonConverter(typeof(IntConverter))]
    [JsonPropertyName("sequence_number")]
    public int? SequenceNumber { get; init; }

    [JsonPropertyName("full_statement")]
    public string? FullStatement { get; init; }

    [JsonPropertyName("node_type")]
    public string? NodeType { get; init; }

    public string? MetadataType { get; init; }

    [JsonConverter(typeof(GuidConverter))]
    [JsonPropertyName("node_type_id")]
    public Guid NodeTypeId { get; init; }

    [JsonPropertyName("item_type")]
    public string? ItemType { get; init; }

    [JsonConverter(typeof(BoolConverter))]
    [JsonPropertyName("project_enabled")]
    public bool? ProjectEnabled { get; init; }

    [JsonPropertyName("project_name")]
    public string? ProjectName { get; init; }

    [JsonConverter(typeof(BoolConverter))]
    [JsonPropertyName("is_document")]
    public bool? IsDocument { get; init; }

    [JsonConverter(typeof(GuidConverter))]
    [JsonPropertyName("parent_id")]
    public Guid ParentId { get; init; }

    [JsonConverter(typeof(GuidConverter))]
    [JsonPropertyName("document_id")]
    public Guid DocumentId { get; init; }

    [JsonPropertyName("document_title")]
    public string? DocumentTitle { get; init; }

    [JsonConverter(typeof(BoolConverter))]
    [JsonPropertyName("is_orphan")]
    public bool? IsOrphan { get; init; }

    [JsonConverter(typeof(IntConverter))]
    public int? Level { get; init; }


    public IImmutableList<Taxonomy> Children { get; init; } = ImmutableList<Taxonomy>.Empty;

}

