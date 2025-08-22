using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;
using integration.Context;

namespace integration.Domain.Entities;

public class Client
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Column("id_asupro")]
    public int IdAsuPro { get; set; }
    
    public string? ExtId { get; set; }
    public string ConsumerName { get; set; } = null!;
    public string? Bik { get; set; }
    public string? MailAddress { get; set; }
    public string? ShortName { get; set; }
    public string? Inn { get; set; }
    public string? Kpp { get; set; }
    public string? Ogrn { get; set; }
    
    [ForeignKey("RootCompany")]
    public Guid? RootCompanyId { get; set; }
    
    [ForeignKey("Boss")]
    public Guid? BossId { get; set; }
    
    public string? PersonId { get; set; }
    public string? DocTypeName { get; set; }
    public string? TypeKa { get; set; }
    public string? Address { get; set; }
    
    // Навигационные свойства
    public RootCompany? RootCompany { get; set; }
    public Boss? Boss { get; set; }

    public List<Contract> Contracts { get; set; } = new();

    // РАЗДЕЛИЛИ на две разные обратные коллекции:
    [InverseProperty(nameof(Location.Participant))]
    public List<Location> ParticipantLocations { get; set; } = new();

    [InverseProperty(nameof(Location.Client))]
    public List<Location> CustomerLocations { get; set; } = new();
    
    // Связь с Boss
}