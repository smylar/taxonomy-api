using System;
using System.Net;
using GraphQL;
using GraphQL.Server.Transports.AspNetCore;
using GraphQL.Types;
using Microsoft.AspNetCore.Mvc;

namespace taxonomy_api.Graphql;

public class HeaderCheckMiddleware<TSchema> : GraphQLHttpMiddleware<TSchema> where TSchema : ISchema
{
    private readonly string _apiKey;

    public HeaderCheckMiddleware(
        RequestDelegate next,
        IGraphQLTextSerializer serializer,
        IDocumentExecuter<TSchema> documentExecuter,
        IServiceScopeFactory serviceScopeFactory,
        GraphQLHttpMiddlewareOptions options,
        IHostApplicationLifetime hostApplicationLifetime)
        : base(next, serializer, documentExecuter, serviceScopeFactory, options, hostApplicationLifetime)
    {
        _apiKey = Environment.GetEnvironmentVariable("API_KEY") ?? string.Empty;
    }


    public override async Task InvokeAsync(HttpContext context)
    {
        var auth = context.Request.Headers.Authorization;

        if (_apiKey.Equals(auth))
        {
            await base.InvokeAsync(context);
        }
        else
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        }
    }
}