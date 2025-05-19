namespace integration.Services.Interfaces;

public interface IService
{
    public Task<HttpClient> CreateAuthorizedClientAsync(AuthType authType);
    public void Message(string ex);
}