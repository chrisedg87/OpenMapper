namespace OpenMapper.Configuration;

internal class MemberConfiguration<TSource, TDestination>
{
    public string DestinationMemberName { get; }
    public Func<TSource, object?> ValueResolver { get; set; }

    public MemberConfiguration(string destinationMemberName)
    {
        DestinationMemberName = destinationMemberName;
        ValueResolver = _ => null;
    }
}
