using System.ComponentModel.DataAnnotations.Schema;
using integration.Context;

namespace integration.Domain.Entities;

public class Emitter
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Amount { get; set; }
    public string? ContractNumber { get; set; }
    public string? LocationMtId { get; set; }
    public string? ExecutorName { get; set; }
    public int IdContract { get; set; }
    public string? ContractStatus { get; set; }
    public int ParticipantId { get; set; }
    public string? TypeConsumer { get; set; }
    public string? NameConsumer { get; set; }
    public string? IdConsumer { get; set; }
    
    [ForeignKey("WasteSource")]
    public Guid WasteSourceId { get; set; }
    
    // Навигационные свойства
    public WasteSource WasteSource { get; set; } = null!;
    public List<ContractPosition> ContractPositions { get; set; } = new();
    public List<Schedule> Schedules { get; set; } = new();
}