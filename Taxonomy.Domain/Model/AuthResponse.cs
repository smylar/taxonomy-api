using System;
using System.Text.Json.Serialization;

namespace API.Taxonomy.Domain.Model;

public class AuthResponse
{
    [JsonPropertyName("access_token")]
    public string? Token { get; set; }
    [JsonPropertyName("expires_in")]
    public int ExpiresInSeconds { get; set; }
}



