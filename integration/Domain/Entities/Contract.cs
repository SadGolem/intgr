using System.ComponentModel.DataAnnotations.Schema;
using integration.Context;

namespace integration.Domain.Entities;

public class Contract
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int ExternalId { get; set; }
    public string Name { get; set; } = null!;
    
    [ForeignKey("Status")]
    public Guid StatusId { get; set; }
    
    public string? RootId { get; set; }
    
    [ForeignKey("Client")]
    public Guid ClientId { get; set; }
    
    public string ContractTypeName { get; set; } = null!;
    
    [ForeignKey("Assignee")]
    public Guid AssigneeId { get; set; }
    
    // Навигационные свойства
    public Status Status { get; set; } = null!;
    public Client Client { get; set; } = null!;
    public Assignee Assignee { get; set; } = null!;
    public List<ContractPosition> Positions { get; set; } = new();
}