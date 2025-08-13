using System.ComponentModel.DataAnnotations;
using integration.Domain.Entities;

public class EmitterEntity : EntityBase
{
    [Required] public int WasteSource_Id { get; set; }
    [MaxLength(500)] public string WasteSource_Address { get; set; }
    [MaxLength(500)] public string WasteSource_Name { get; set; }
    [MaxLength(100)] public string WasteSource_Category { get; set; }
    public bool WasteSource_Normative { get; set; }
    [MaxLength(100)] public string WasteSource_Ext_id { get; set; }
    public List<int>? Containers_IDs { get; set; }
    public decimal? Amount { get; set; }
    [MaxLength(200)] public string ContractNumber { get; set; }
    [MaxLength(100)] public string Location_Mt_Id { get; set; }
    [MaxLength(200)] public string ExecutorName { get; set; }
    public int IdContract { get; set; }
    public string ContractStatus { get; set; }
    public int Participant_Id { get; set; }
    public string TypeConsumer { get; set; }
    public string NameConsumer { get; set; }
    public string IdConsumer { get; set; }
    public ICollection<ScheduleEntity> Schedules { get; set; } = new List<ScheduleEntity>();
}