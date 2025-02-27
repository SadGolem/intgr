using System.Text.Json.Serialization;

namespace integration.Context;

public class ContractPositionData : Data
{
    [JsonPropertyName("id")] //позиция договора
    public int id { get; set; }
    
    [JsonPropertyName("number")]
    public string number { get; set; }
    
    [JsonPropertyName("status")]
    public Status status { get; set; }
    
    [JsonPropertyName("waste_source")]
    public EmitterData waste_source { get; set; }
    
    [JsonPropertyName("waste_site")]
    public LocationData waste_site { get; set; }
    
    [JsonPropertyName("value")]
    public string value { get; set; }
    
    [JsonPropertyName("value_manual")]
    public string value_manual { get; set; }
    
    [JsonPropertyName("contract")]
    public ContractData contract { get; set; }
}