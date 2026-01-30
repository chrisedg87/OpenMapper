using OpenMapper.Configuration;

namespace OpenMapper.Core;

public abstract class Profile
{
    internal List<TypeMapConfiguration> TypeMapConfigurations { get; } = new();

    protected IMappingExpression<TSource, TDestination> CreateMap<TSource, TDestination>()
    {
        var config = new TypeMapConfiguration(typeof(TSource), typeof(TDestination));
        TypeMapConfigurations.Add(config);
        return new MappingExpression<TSource, TDestination>();
    }

    private class MappingExpression<TSource, TDestination> : IMappingExpression<TSource, TDestination>
    {
    }
}
