using System.Reflection;

namespace OpenMapper.Execution;

internal class TypeMap
{
    private readonly Type _sourceType;
    private readonly Type _destinationType;
    private readonly List<PropertyMap> _propertyMaps;

    public TypeMap(Type sourceType, Type destinationType)
    {
        _sourceType = sourceType;
        _destinationType = destinationType;
        _propertyMaps = DiscoverPropertyMaps();
    }

    private List<PropertyMap> DiscoverPropertyMaps()
    {
        var propertyMaps = new List<PropertyMap>();

        var sourceProperties = _sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var destinationProperties = _destinationType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        var destPropertiesDict = destinationProperties
            .Where(p => p.CanWrite)
            .ToDictionary(p => p.Name);

        foreach (var sourceProperty in sourceProperties)
        {
            if (!sourceProperty.CanRead)
                continue;

            if (destPropertiesDict.TryGetValue(sourceProperty.Name, out var destinationProperty))
            {
                if (destinationProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
                {
                    propertyMaps.Add(new PropertyMap(sourceProperty, destinationProperty));
                }
            }
        }

        return propertyMaps;
    }

    public object? Map(object? source)
    {
        if (source == null)
            return null;

        var destination = Activator.CreateInstance(_destinationType)
            ?? throw new InvalidOperationException($"Cannot create instance of type {_destinationType.Name}");

        foreach (var propertyMap in _propertyMaps)
        {
            propertyMap.Map(source, destination);
        }

        return destination;
    }

    public object Map(object? source, object destination)
    {
        if (source == null)
            return destination;

        foreach (var propertyMap in _propertyMaps)
        {
            propertyMap.Map(source, destination);
        }

        return destination;
    }
}
