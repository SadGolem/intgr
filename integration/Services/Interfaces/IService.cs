namespace integration.Services.Interfaces;

public interface IService
{
    protected Task Authorize(HttpClient httpClient);
    protected void Message(string ex);
}