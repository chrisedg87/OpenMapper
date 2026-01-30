using System.Reflection;

namespace OpenMapper.Execution;

internal class PropertyMap
{
    public PropertyInfo SourceProperty { get; }
    public PropertyInfo DestinationProperty { get; }

    public PropertyMap(PropertyInfo sourceProperty, PropertyInfo destinationProperty)
    {
        SourceProperty = sourceProperty;
        DestinationProperty = destinationProperty;
    }

    public void Map(object source, object destination)
    {
        var value = SourceProperty.GetValue(source);
        DestinationProperty.SetValue(destination, value);
    }
}
