namespace integration.Context.Request;

public class EntryRequest
{
    public long idAsuPro { get; set; }
    public string? consumerName { get; set; }
    public string? creator { get; set; } //author
    public string? status { get; set; }
    public int idLocation { get; set; } // ext_id
    public int amount { get; set; } //kol-vo ainerov
    public float volume { get; set; } 
    public string? creationDate { get; set; } 
    public string? planDateRO { get; set; } 
    public string? commentByRO { get; set; } 
    public string type { get; set; } 
    public int idContainerType { get; set; } 
}