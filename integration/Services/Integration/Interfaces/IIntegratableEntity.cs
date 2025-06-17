namespace integration.Services.Integration.Interfaces;

public interface IIntegratableEntity
{
    int GetIntegrationExtId();
    void UpdateIntegrationId(int newId);
}