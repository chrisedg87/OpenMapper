namespace OpenMapper.Execution;

internal abstract class PropertyMap
{
    public string DestinationPropertyName { get; }

    protected PropertyMap(string destinationPropertyName)
    {
        DestinationPropertyName = destinationPropertyName;
    }

    public abstract void Map(object source, object destination);
}
