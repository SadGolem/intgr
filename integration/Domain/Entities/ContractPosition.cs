using System.ComponentModel.DataAnnotations.Schema;
using integration.Context;

namespace integration.Domain.Entities;

public class ContractPosition
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int ExternalId { get; set; }
    public string Number { get; set; } = null!;
    
    [ForeignKey("Status")]
    public Guid StatusId { get; set; }
    
    public double? Value { get; set; }
    public double? ValueManual { get; set; }
    public double? EstimationValue { get; set; }
    public DateTime DateStart { get; set; }
    public DateTime DateEnd { get; set; }
    
    [ForeignKey("Contract")]
    public Guid ContractId { get; set; }
    
    [ForeignKey("Emitter")]
    public Guid EmitterId { get; set; }
    
    [ForeignKey("Location")]
    public Guid LocationId { get; set; }
    
    // Навигационные свойства
    public Status Status { get; set; } = null!;
    public Contract Contract { get; set; } = null!;
    public Emitter Emitter { get; set; } = null!;
    public Location Location { get; set; } = null!;
    public List<Entry> Entries { get; set; } = new();
}