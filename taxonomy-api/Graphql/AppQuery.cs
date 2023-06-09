using System;
using System.Security.Cryptography;
using API.Taxonomy.Domain.Model;
using taxonomy_api.Repository;
using GraphQL;
using GraphQL.Types;

namespace taxonomy_api.Graphql;

public class AppQuery : ObjectGraphType
{
    private readonly ITaxonomyRepository _repository;

    public AppQuery(ITaxonomyRepository repository)
    {
        _repository = repository;

        Field<TaxonomyType>("index")
            .Argument<ListGraphType<StringGraphType>>("nodeType")
            .Resolve(context =>
            {
                var nodeFilter = context.GetArgument<IEnumerable<string>>("nodeType");
                return _repository.GetIndex(nodeFilter);
            });

        Field<TaxonomyType>("taxonomy")
            .Argument<NonNullGraphType<IdGraphType>>("id")
            .Argument<ListGraphType<StringGraphType>>("nodeType")
            .Argument<IdGraphType>("documentId")
            .Resolve(context =>
            {
                var nodeFilter = context.GetArgument<IEnumerable<string>>("nodeType");
                var id = context.GetArgument<Guid>("id");
                var documentId = context.GetArgument<Guid>("documentId");

                return documentId != Guid.Empty && !documentId.Equals(id)
                                    ? _repository.GetChildById(documentId, id, nodeFilter)
                                    : _repository.GetById(id, nodeFilter);
            });

        Field<ListGraphType<TaxonomyType>>("linked_taxonomy")
            .Argument<ListGraphType<StringGraphType>>("nodeType")
            .Argument<NonNullGraphType<IdGraphType>>("id")
            .Resolve(context =>
            {
                var nodeFilter = context.GetArgument<IEnumerable<string>>("nodeType");
                var id = context.GetArgument<Guid>("id");
                return _repository.GetByAssociation(id, nodeFilter);
            });
    }
}

