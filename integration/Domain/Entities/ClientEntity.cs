using System.ComponentModel.DataAnnotations;
using integration.Context;

namespace integration.Domain.Entities;

public class ClientEntity : EntityBase
{
    [Required] public int IdAsuPro { get; set; }
    [MaxLength(100)] public string? Ext_id { get; set; }
    [MaxLength(100)] public string ConsumerName { get; set; }
    [MaxLength(100)] public string Bik { get; set; }
    [MaxLength(100)] public string MailAddress { get; set; }
    [MaxLength(100)] public string ShortName { get; set; }
    [MaxLength(100)] public string Inn { get; set; }
    [MaxLength(100)] public string Kpp { get; set; }
    [MaxLength(100)] public string Ogrn { get; set; }
    [MaxLength(100)] public string Root_company { get; set; }
    [MaxLength(100)] public string Boss { get; set; }
    [MaxLength(100)] public string Person_id { get; set; }
    [Required] [MaxLength(100)] public string Doc_type { get; set; }
    [Required] [MaxLength(100)] public string Type_ka { get; set; }
    [MaxLength(500)] public string Address { get; set; }
}