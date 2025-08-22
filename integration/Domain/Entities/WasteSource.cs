using System.ComponentModel.DataAnnotations.Schema;
using integration.Context;

namespace integration.Domain.Entities;

public class WasteSource
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int ExternalId { get; set; }
    public string Address { get; set; } = null!;
    public string Name { get; set; } = null!;
    public bool Normative { get; set; }
    
    [ForeignKey("Category")]
    public Guid CategoryId { get; set; }
    
    // Навигационные свойства
    public WasteSourceCategory Category { get; set; } = null!;
    public List<Emitter> Emitters { get; set; } = new();
}