using System.ComponentModel.DataAnnotations.Schema;
using integration.Context;

namespace integration.Domain.Entities;

public class Location
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int ExternalId { get; set; }
    public decimal Lon { get; set; }
    public decimal Lat { get; set; }
    public string Address { get; set; } = null!;
    public string? Comment { get; set; }
    public string? AuthorUpdate { get; set; }
    
    [ForeignKey("Status")]
    public Guid StatusId { get; set; }
    
    [ForeignKey("Participant")]
    public Guid? ParticipantId { get; set; }
    
    [ForeignKey("Client")]
    public Guid? ClientId { get; set; }
    
    // Навигационные свойства
    public Status Status { get; set; } = null!;
    public Client? Participant { get; set; }
    public Client? Client { get; set; }
    public List<ContractPosition> ContractPositions { get; set; } = new();
    public List<Entry> Entries { get; set; } = new();
    public List<Schedule> Schedules { get; set; } = new();
}