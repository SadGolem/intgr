using System.Net;

namespace integration.Exceptions;

public class ApiException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public string Content { get; }
    
    public ApiException(HttpStatusCode statusCode, string content)
        : base($"API error: {statusCode}")
    {
        StatusCode = statusCode;
        Content = content;
    }
}