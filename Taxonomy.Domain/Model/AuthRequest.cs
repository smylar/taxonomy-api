using System;
using System.Text.Json.Serialization;

namespace API.Taxonomy.Domain.Model;

public class AuthRequest
{
    public string ClientId { get; } = Environment.GetEnvironmentVariable("FROST_USER") ?? String.Empty;
    public string ClientSecret { get; } = Environment.GetEnvironmentVariable("FROST_PASS") ?? String.Empty;
}

