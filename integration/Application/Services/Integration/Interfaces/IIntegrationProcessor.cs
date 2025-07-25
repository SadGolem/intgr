namespace integration.Services.Integration.Interfaces;

public interface IIntegrationProcessor<T> where T : IIntegratableEntity
{
    Task ProcessAsync(T entity);
}