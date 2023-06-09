using System;
using System.Collections.Immutable;
using System.Reflection;
using AutoMapper;
using API.Taxonomy.Domain.Model;

namespace taxonomy_api.Repository;

public class AutoMapProfile : Profile
{
    public AutoMapProfile()
    {
        CreateMap<Taxonomy, Taxonomy>()
            .ForMember(d => d.Children, opt =>
             {
                 opt.AllowNull();
                 opt.MapFrom((src, dst, _, context) => ImmutableList.ToImmutableList(src.Children.Where(t => Filter(t, context))));
             });

        CreateMap<IImmutableList<Taxonomy>, IImmutableList<Taxonomy>>()
            .ConstructUsing((src, ctx) => ImmutableList.ToImmutableList(src.Select(t => ctx.Mapper.Map<Taxonomy>(t))));
    }

    private bool Filter(Taxonomy taxonomy, ResolutionContext context) 
    {
        var filterKey = TaxonomyRepository.filterKey;
        if (!context.Items.ContainsKey(filterKey) || context.Items[filterKey] == null || !typeof(IEnumerable<string>).IsAssignableFrom(context.Items[filterKey].GetType()))
        {
            return true;
        }

        if (taxonomy.NodeType == null)
        {
            return false;
        }

        var filter = (IEnumerable<string>)context.Items[filterKey];

        return filter.Count() == 0 ? true : filter.Select(f => f.ToLower())
                                             .Contains(taxonomy.NodeType.ToLower());

    }
}

