using integration.Helpers.Auth;

namespace integration.Services.Token.Interfaces;

public interface ITokenService
{
    Task<string> GetTokenAsync(IAuth settings);
}