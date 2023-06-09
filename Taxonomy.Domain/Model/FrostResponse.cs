using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace API.Taxonomy.Domain.Model;

public class FrostResponse
{
    public int Status { get; set; }
    public string? Message { get; set; }
    public bool? Success { get; set; }
    public DataObject? Data { get; set; }
}

public class DataObject
{
    public IEnumerable<Taxonomy>? Children { get; set; }

    [JsonPropertyName("import_type")]
    public int ImportType { get; set; }

    [JsonPropertyName("is_deleted")]
    public int IsDeleted { get; set; }
}

