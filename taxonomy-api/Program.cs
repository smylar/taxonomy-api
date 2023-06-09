
using System.Net.Http.Headers;
using taxonomy_api.Repository;
using taxonomy_api.HttpService;
using GraphQL;
using GraphQL.Server.Transports.AspNetCore;
using taxonomy_api.Graphql;
using taxonomy_api.Health;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

string frostUrl = Environment.GetEnvironmentVariable("FROST_URL") ?? string.Empty;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient<FrostHttpService>(client => {
    client.BaseAddress = new Uri(frostUrl);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

});
// Add services to the container.

builder.Services.AddSingleton<ITaxonomyRepository, TaxonomyRepository>()
    .AddSingleton<TaxonomyCache>()
    .AddSingleton<FrostTokenStore>()
    .AddGraphQL(c =>
    {
        c.AddSystemTextJson();
        c.AddSchema<AppSchema>();
        c.AddGraphTypes();
    })
    .AddAutoMapper(typeof(AutoMapProfile))
    .AddApplicationInsightsTelemetry()
    .AddHostedService<TaxonomyIndex>()
    .AddHostedService<IndexAssociationService>()
    .AddSingleton<StartupHealthCheck>()
    .AddHealthChecks()
        .AddCheck<StartupHealthCheck>("Index", tags: new[] { "ready" });

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseGraphQL<HeaderCheckMiddleware<AppSchema>>("/graphql", new GraphQLHttpMiddlewareOptions());

app.MapHealthChecks("/healthcheck/ready", new HealthCheckOptions
{
    Predicate = healthCheck => healthCheck.Tags.Contains("ready")
});

app.MapHealthChecks("/healthcheck/live", new HealthCheckOptions
{
    Predicate = _ => false
});

app.UseHttpsRedirection();

app.Run();

