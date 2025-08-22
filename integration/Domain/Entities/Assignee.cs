namespace integration.Domain.Entities;

public class Assignee
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int ExternalId { get; set; }
    public string Name { get; set; } = null!;
    public List<Contract> Contracts { get; set; } = new();
}