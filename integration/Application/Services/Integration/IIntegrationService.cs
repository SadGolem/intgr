using integration.Structs;

namespace integration.Services.Integration;

public interface IIntegrationService
{
    public Task SendIntegrationDataAsync(IntegrationStruct _struct);
}