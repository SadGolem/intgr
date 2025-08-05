using integration.Structs;

namespace integration.Services.Integration;

public interface IIntegrationService
{
    Task SendIntegrationDataAsync(IntegrationStruct integrationStruct);
    Task<bool> ValidateIntegrationDataAsync(IntegrationStruct integrationStruct);
}