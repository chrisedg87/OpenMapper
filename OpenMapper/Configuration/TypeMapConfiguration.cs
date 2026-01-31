namespace OpenMapper.Configuration;

internal class TypeMapConfiguration
{
    public Type SourceType { get; }
    public Type DestinationType { get; }
    public Dictionary<string, object> MemberConfigurations { get; } = new();

    public TypeMapConfiguration(Type sourceType, Type destinationType)
    {
        SourceType = sourceType;
        DestinationType = destinationType;
    }

    public void AddMemberConfiguration<TSource, TDestination>(
        string destinationMemberName,
        MemberConfiguration<TSource, TDestination> memberConfiguration)
    {
        MemberConfigurations[destinationMemberName] = memberConfiguration;
    }
}
