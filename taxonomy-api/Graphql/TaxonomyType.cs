using System;
using API.Taxonomy.Domain.Model;
using GraphQL.Types;

namespace taxonomy_api.Graphql;

public class TaxonomyType: ObjectGraphType<Taxonomy>
{
    public TaxonomyType()
    {
        Field(t => t.Id, type: typeof(IdGraphType)).Description("Id property from the taxonomy object.");
        Field(t => t.Title).Description("Title of the taxonomy object.");
        Field(t => t.HumanCodingScheme);
        Field(t => t.ListEnumeration);
        Field(t => t.SequenceNumber, type: typeof(IntGraphType));
        Field(t => t.FullStatement).Description("Full description of the taxonomy object.");
        Field(t => t.NodeType).Description("The taxonomy type Unit/Journey etc");
        Field(t => t.MetadataType);
        Field(t => t.NodeTypeId, type: typeof(IdGraphType));
        Field(t => t.ItemType);
        Field(t => t.ProjectEnabled, type: typeof(BooleanGraphType));
        Field(t => t.ProjectName);
        Field(t => t.IsDocument, type: typeof(BooleanGraphType));
        Field(t => t.ParentId, type: typeof(IdGraphType)).Description("ID of parent taxonomy"); ;
        Field(t => t.DocumentId, type: typeof(IdGraphType));
        Field(t => t.DocumentTitle);
        Field(t => t.IsOrphan, type: typeof(BooleanGraphType));
        Field(t => t.Level, type: typeof(IntGraphType));
        Field(t => t.Children, type: typeof(ListGraphType<TaxonomyType>)).Description("Sub taxonomies this is parent of");
    }
}

