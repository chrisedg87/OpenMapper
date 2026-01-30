namespace OpenMapper.Exceptions;

public class MappingNotFoundException : OpenMapperException
{
    public Type SourceType { get; }
    public Type DestinationType { get; }

    public MappingNotFoundException(Type sourceType, Type destinationType)
        : base($"Missing mapping configuration from {sourceType.Name} to {destinationType.Name}. " +
               $"Ensure you have configured this mapping using CreateMap<{sourceType.Name}, {destinationType.Name}>() in a Profile.")
    {
        SourceType = sourceType;
        DestinationType = destinationType;
    }
}
