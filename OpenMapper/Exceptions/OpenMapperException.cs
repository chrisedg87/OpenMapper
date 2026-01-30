namespace OpenMapper.Exceptions;

public class OpenMapperException : Exception
{
    public OpenMapperException() : base()
    {
    }

    public OpenMapperException(string message) : base(message)
    {
    }

    public OpenMapperException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
