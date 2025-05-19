namespace integration.Helpers.Interfaces;

public interface IAuthorizer
{
    Task<string> GetCachedTokenMTAsync();
    Task<string> GetCachedTokenAPROAsync();
}