namespace OpenMapper.Configuration;

internal class TypeMapConfiguration
{
    public Type SourceType { get; }
    public Type DestinationType { get; }

    public TypeMapConfiguration(Type sourceType, Type destinationType)
    {
        SourceType = sourceType;
        DestinationType = destinationType;
    }
}
