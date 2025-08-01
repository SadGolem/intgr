using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace integration.Domain.Entities;

public class LocationEntity : EntityBase
{
    [Required]
    public int IdAsuPro { get; set; } 

    [Required]
    [Column(TypeName = "varchar(20)")]
    public string Status { get; set; } = "UNKNOWN";

    [Required]
    [Column(TypeName = "decimal(9,6)")]
    public decimal Longitude { get; set; }

    [Required]
    [Column(TypeName = "decimal(9,6)")]
    public decimal Latitude { get; set; }

    [Required]
    [MaxLength(500)]
    public string Address { get; set; } = string.Empty; 

    [MaxLength(100)]
    public string? ExtId { get; set; }

    [MaxLength(1000)]
    public string? Comment { get; set; }
    
    public int? IdParticipant { get; set; }
    
    public int? IdClient { get; set; }

    [MaxLength(100)]
    public string? AuthorUpdate { get; set; }
}
