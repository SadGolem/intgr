namespace integration.Context.Request;

public class EntryRequest
{
    public int? idAsuPro { get; set; }
    public string? consumerName { get; set; }
    public string? creator { get; set; } //author
    public string? status { get; set; }
    public int? idLocation { get; set; } // ext_id
    public int? amount { get; set; } //kol-vo containerov
    public float? volume { get; set; } 
    public string? creationDate { get; set; } 
    public string? planDateRO { get; set; } 
    public string? commentByRO { get; set; } 
    public string? type { get; set; } 
    public string? idContainerType { get; set; } 
}