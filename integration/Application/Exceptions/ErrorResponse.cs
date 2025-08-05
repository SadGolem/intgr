namespace integration.Exceptions;

public class ErrorResponse
{
    public string timeStamp { get; set; }
    public string errorMessage { get; set; }
    public string httpStatusCode { get; set; }
    public string method { get; set; }
    public string url { get; set; }
    public string remoteUser { get; set; }
    public string exceptionClass { get; set; }
}