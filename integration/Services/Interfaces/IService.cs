namespace integration.Services.Interfaces;

public interface IService
{
    protected Task Authorize(HttpClient httpClient, bool isApro);
    protected void Message(string ex);
}