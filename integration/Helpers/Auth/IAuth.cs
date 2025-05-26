namespace integration.Helpers.Auth;

public interface IAuth
{
    public string Login { get; set; }
    public string Password { get; set; }
    public string CallbackUrl { get; set; }
    public string BaseUrl { get; set; }
    
    public ApiClientSettings ApiClientSettings { get; set; }
}