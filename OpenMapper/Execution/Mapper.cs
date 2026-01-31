using OpenMapper.Core;
using OpenMapper.Exceptions;
using System.Collections.Generic;

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

        // Check if we're mapping a collection
        if (IsEnumerableType(sourceType, out var sourceElementType) &&
            IsEnumerableType(destinationType, out var destElementType))
        {
            return (TDestination?)MapCollection(source, sourceElementType!, destElementType!);
        }

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

    private bool IsEnumerableType(Type type, out Type? elementType)
    {
        // Check if it's a generic type
        if (type.IsGenericType)
        {
            var genericTypeDef = type.GetGenericTypeDefinition();

            // Check for List<T>
            if (genericTypeDef == typeof(List<>))
            {
                elementType = type.GetGenericArguments()[0];
                return true;
            }

            // Check for IEnumerable<T>
            if (genericTypeDef == typeof(IEnumerable<>))
            {
                elementType = type.GetGenericArguments()[0];
                return true;
            }
        }

        // Check if it implements IEnumerable<T>
        var enumerableInterface = type.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType &&
                               i.GetGenericTypeDefinition() == typeof(IEnumerable<>));

        if (enumerableInterface != null)
        {
            elementType = enumerableInterface.GetGenericArguments()[0];
            return true;
        }

        elementType = null;
        return false;
    }

    private object MapCollection(object source, Type sourceElementType, Type destElementType)
    {
        var sourceEnumerable = (System.Collections.IEnumerable)source;
        var destListType = typeof(List<>).MakeGenericType(destElementType);
        var destList = (System.Collections.IList)Activator.CreateInstance(destListType)!;

        foreach (var item in sourceEnumerable)
        {
            if (item == null)
            {
                destList.Add(null);
            }
            else
            {
                var typePair = new TypePair(sourceElementType, destElementType);

                if (!_typeMaps.TryGetValue(typePair, out var typeMap))
                {
                    throw new MappingNotFoundException(sourceElementType, destElementType);
                }

                var mappedItem = typeMap.Map(item);
                destList.Add(mappedItem);
            }
        }

        return destList;
    }
}
