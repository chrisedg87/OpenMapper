using System.Reflection;

namespace OpenMapper.Execution;

internal class CustomPropertyMap<TSource, TDestination> : PropertyMap
{
    private readonly Func<TSource, object?> _valueResolver;
    private readonly PropertyInfo _destinationProperty;

    public CustomPropertyMap(
        string destinationPropertyName,
        Func<TSource, object?> valueResolver,
        PropertyInfo destinationProperty)
        : base(destinationPropertyName)
    {
        _valueResolver = valueResolver;
        _destinationProperty = destinationProperty;
    }

    public override void Map(object source, object destination)
    {
        if (source is not TSource typedSource)
            return;

        var value = _valueResolver(typedSource);
        var convertedValue = ConvertValue(value, _destinationProperty.PropertyType);
        _destinationProperty.SetValue(destination, convertedValue);
    }

    private static object? ConvertValue(object? value, Type targetType)
    {
        if (value == null)
            return null;

        var valueType = value.GetType();

        // If types match or target can accept source, no conversion needed
        if (targetType.IsAssignableFrom(valueType))
            return value;

        // Handle nullable types
        var underlyingType = Nullable.GetUnderlyingType(targetType);
        if (underlyingType != null)
        {
            if (underlyingType.IsAssignableFrom(valueType))
                return value;

            targetType = underlyingType;
        }

        // Try Convert.ChangeType for primitives and common types
        try
        {
            return Convert.ChangeType(value, targetType);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Cannot convert value of type {valueType.Name} to {targetType.Name}", ex);
        }
    }
}
