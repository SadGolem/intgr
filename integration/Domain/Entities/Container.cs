using System.ComponentModel.DataAnnotations.Schema;

namespace integration.Domain.Entities;

public class Container
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int? ExternalId { get; set; }
    public int? TypeId { get; set; }
    public int? CapacityId { get; set; }
    
    [ForeignKey("Schedule")]
    public Guid ScheduleId { get; set; }
    
    // Навигационные свойства
    public Schedule Schedule { get; set; } = null!;
}