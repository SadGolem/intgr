using System.ComponentModel.DataAnnotations;

namespace integration.Domain.Entities;

public class ScheduleEntity : EntityBase
{
    [Required] public int IdAsuPro { get; set; }
    [Required] public int IdLocation { get; set; }
    public List<int>? Containers_IDs { get; set; }
    [Required] [MaxLength(200)] public string Schedule { get; set; }
    [MaxLength(500)] public string[] Dates { get; set; }
    [MaxLength(100)] public string? Ext_id;
    public int? IdEmitter { get; set; }
    public int? idContainerType { get; set; }
}