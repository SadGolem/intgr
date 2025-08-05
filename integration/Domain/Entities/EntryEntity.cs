using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace integration.Domain.Entities;

public class EntryEntity : EntityBase
{
    [Required] 
    public int IsAsuPro { get; set; }

    [MaxLength(500)] 
    public string? PlanDateRO { get; set; }

    [MaxLength(200)] 
    public string Author { get; set; }
    
    [Required] 
    public int IdLocation { get; set; }

    [MaxLength(200)] 
    public string Status { get; set; }

    [MaxLength(200)]
    public string Agreement { get; set; }

    [MaxLength(500)] 
    public string? Comment { get; set; }
    
    [Column(TypeName = "float")] 
    public float? Volume { get; set; }  

    [Column(TypeName = "float")] 
    public float? Capacity { get; set; }

    public int? Count { get; set; }

    public int? IdContainerType { get; set; }  
}