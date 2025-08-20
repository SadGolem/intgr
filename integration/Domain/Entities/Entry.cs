using System.ComponentModel.DataAnnotations.Schema;

namespace integration.Domain.Entities;

public class Entry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int BtNumber { get; set; }
    public DateTime PlanDateRo { get; set; }
    public int StatusId { get; set; }
    public string? Comment { get; set; }
    public float? Volume { get; set; }
    public int? Number { get; set; }
    public int? IdContainerType { get; set; }
    public string? StatusString { get; set; }
    
    [ForeignKey("Location")]
    public Guid LocationId { get; set; }
    
    public int AgreementId { get; set; }
    public int CapacityId { get; set; }
    public string? AuthorName { get; set; }
    
    // Навигационные свойства
    public Location Location { get; set; } = null!;
}