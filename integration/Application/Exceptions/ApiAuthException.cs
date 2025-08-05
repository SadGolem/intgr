namespace integration.Exceptions;

public class ApiAuthException : Exception
{
    public ApiAuthException(string message) : base(message) {}
    public ApiAuthException(string message, Exception inner) : base(message, inner) {}
}