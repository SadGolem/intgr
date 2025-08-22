namespace integration.Domain.Entities;

public class RootCompany
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int ExternalId { get; set; }
    
    // Навигационное свойство для связи с клиентами
    public List<Client> Clients { get; set; } = new List<Client>();
}