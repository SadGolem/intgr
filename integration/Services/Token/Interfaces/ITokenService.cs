using integration.HelpClasses;

namespace integration.Services.Token.Interfaces;

public interface ITokenService
{
    Task<string> GetTokenAsync(AuthSettings settings);
}