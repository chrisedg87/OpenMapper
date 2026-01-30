using OpenMapper.Core;
using OpenMapper.Exceptions;

namespace OpenMapper.Execution;

internal class Mapper : IMapper
{
    private readonly Dictionary<TypePair, TypeMap> _typeMaps;

    public Mapper(Dictionary<TypePair, TypeMap> typeMaps)
    {
        _typeMaps = typeMaps;
    }

    public TDestination? Map<TDestination>(object? source)
    {
        if (source == null)
            return default;

        var sourceType = source.GetType();
        var destinationType = typeof(TDestination);
        var typePair = new TypePair(sourceType, destinationType);

        if (!_typeMaps.TryGetValue(typePair, out var typeMap))
        {
            throw new MappingNotFoundException(sourceType, destinationType);
        }

        return (TDestination?)typeMap.Map(source);
    }

    public TDestination? Map<TSource, TDestination>(TSource? source)
    {
        if (source == null)
            return default;

        var sourceType = typeof(TSource);
        var destinationType = typeof(TDestination);
        var typePair = new TypePair(sourceType, destinationType);

        if (!_typeMaps.TryGetValue(typePair, out var typeMap))
        {
            throw new MappingNotFoundException(sourceType, destinationType);
        }

        return (TDestination?)typeMap.Map(source);
    }

    public TDestination Map<TSource, TDestination>(TSource? source, TDestination destination)
    {
        if (destination == null)
            throw new ArgumentNullException(nameof(destination));

        if (source == null)
            return destination;

        var sourceType = typeof(TSource);
        var destinationType = typeof(TDestination);
        var typePair = new TypePair(sourceType, destinationType);

        if (!_typeMaps.TryGetValue(typePair, out var typeMap))
        {
            throw new MappingNotFoundException(sourceType, destinationType);
        }

        return (TDestination)typeMap.Map(source, destination);
    }
}
