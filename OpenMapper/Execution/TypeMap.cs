using System.Reflection;
using OpenMapper.Configuration;

namespace OpenMapper.Execution;

internal class TypeMap
{
    private readonly Type _sourceType;
    private readonly Type _destinationType;
    private readonly List<PropertyMap> _propertyMaps;

    public TypeMap(TypeMapConfiguration config)
    {
        _sourceType = config.SourceType;
        _destinationType = config.DestinationType;
        _propertyMaps = BuildPropertyMaps(config);
    }

    private List<PropertyMap> BuildPropertyMaps(TypeMapConfiguration config)
    {
        var propertyMaps = new List<PropertyMap>();
        var destinationProperties = _destinationType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var destPropertiesDict = destinationProperties
            .Where(p => p.CanWrite)
            .ToDictionary(p => p.Name);

        // Phase 1: Create CustomPropertyMap for each ForMember configuration
        foreach (var kvp in config.MemberConfigurations)
        {
            var memberName = kvp.Key;
            var memberConfig = kvp.Value;

            // Validate that the destination property exists
            if (!destPropertiesDict.TryGetValue(memberName, out var destinationProperty))
            {
                throw new InvalidOperationException(
                    $"Property '{memberName}' does not exist on type {_destinationType.Name}");
            }

            // Create CustomPropertyMap using reflection to call generic constructor
            var memberConfigType = memberConfig.GetType();
            var genericArgs = memberConfigType.GetGenericArguments();

            var customPropertyMapType = typeof(CustomPropertyMap<,>).MakeGenericType(genericArgs);

            var valueResolverProperty = memberConfigType.GetProperty("ValueResolver");
            var valueResolver = valueResolverProperty?.GetValue(memberConfig);

            var customPropertyMap = (PropertyMap)Activator.CreateInstance(
                customPropertyMapType,
                memberName,
                valueResolver,
                destinationProperty)!;

            propertyMaps.Add(customPropertyMap);
        }

        // Phase 2: Auto-discover remaining properties (skip those already customized)
        var customizedProperties = new HashSet<string>(config.MemberConfigurations.Keys);
        var sourceProperties = _sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var sourceProperty in sourceProperties)
        {
            if (!sourceProperty.CanRead)
                continue;

            // Skip if already customized with ForMember
            if (customizedProperties.Contains(sourceProperty.Name))
                continue;

            if (destPropertiesDict.TryGetValue(sourceProperty.Name, out var destinationProperty))
            {
                if (destinationProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
                {
                    propertyMaps.Add(new ReflectionPropertyMap(sourceProperty, destinationProperty));
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
