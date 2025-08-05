namespace integration.Exceptions;

public class DuplicateLocationException: InvalidOperationException
{
    public DuplicateLocationException(string message, Exception inner) 
        : base(message, inner) { }
}