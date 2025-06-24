using System.Net;

namespace integration.Exceptions;

public class ApiException : Exception
{
    public ApiException(string message, string details) 
        : base(message) 
    {
        Details = details;
    }
    
    public string Details { get; }
}