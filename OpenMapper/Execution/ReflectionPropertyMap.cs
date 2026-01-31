using System.Reflection;

namespace OpenMapper.Execution;

internal class ReflectionPropertyMap : PropertyMap
{
    private readonly PropertyInfo _sourceProperty;
    private readonly PropertyInfo _destinationProperty;

    public ReflectionPropertyMap(PropertyInfo sourceProperty, PropertyInfo destinationProperty)
        : base(destinationProperty.Name)
    {
        _sourceProperty = sourceProperty;
        _destinationProperty = destinationProperty;
    }

    public override void Map(object source, object destination)
    {
        var value = _sourceProperty.GetValue(source);
        _destinationProperty.SetValue(destination, value);
    }
}
