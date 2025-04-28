namespace integration;

public interface ITokenService
{
    Task<string> GetCachedTokenMT();
    Task<string> GetCachedTokenAPRO();
}