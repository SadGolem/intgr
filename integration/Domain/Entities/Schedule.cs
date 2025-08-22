using System.ComponentModel.DataAnnotations.Schema;

namespace integration.Domain.Entities;

public class Schedule
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string GrW { get; set; } = null!;
    public string[] Dates { get; set; } = Array.Empty<string>();
    public string? ExtId { get; set; }
    public int? IdContainerType { get; set; }
    
    [ForeignKey("Location")]
    public Guid LocationId { get; set; }
    
    [ForeignKey("Emitter")]
    public Guid EmitterId { get; set; }
    
    // Навигационные свойства
    public Location Location { get; set; } = null!;
    public Emitter Emitter { get; set; } = null!;
    public List<Container> Containers { get; set; } = new();
}