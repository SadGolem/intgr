namespace integration.Domain.Entities;

public class Status
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int ExternalId { get; set; }
    public string Name { get; set; } = null!;
    
    // Для фильтрации (договор/позиция/локация)
    public string EntityType { get; set; } = null!; 
}