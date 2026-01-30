namespace OpenMapper.Execution;

internal readonly struct TypePair : IEquatable<TypePair>
{
    public Type Source { get; }
    public Type Destination { get; }

    public TypePair(Type source, Type destination)
    {
        Source = source;
        Destination = destination;
    }

    public bool Equals(TypePair other)
    {
        return Source == other.Source && Destination == other.Destination;
    }

    public override bool Equals(object? obj)
    {
        return obj is TypePair other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Source, Destination);
    }
}
