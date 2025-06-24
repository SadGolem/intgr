using System.Net;

namespace integration.Exceptions;

public class ApiException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public string ResponseBody { get; }
    public string ErrorMessage { get; }

    public ApiException(
        string message,
        HttpStatusCode statusCode,
        string responseBody,
        string errorMessage = null
    ) : base(message)
    {
        StatusCode = statusCode;
        ResponseBody = responseBody;
        ErrorMessage = errorMessage;
    }
}