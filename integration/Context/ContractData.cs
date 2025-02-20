using System.Text.Json.Serialization;

namespace integration.Context;

public class ContractData : Data
{
    [JsonPropertyName("id")]
    public int id { get; set; }
    
    [JsonPropertyName("number")]
    public string number { get; set; }
    
    [JsonPropertyName("waste_source")]
    public EmitterData waste_source { get; set; }
    
    [JsonPropertyName("waste_site")]
    public LocationData waste_site { get; set; }
    
    [JsonPropertyName("estimation_value")]
    public string estimation_value { get; set; }
    
    [JsonPropertyName("value")]
    public string value { get; set; }
    
    [JsonPropertyName("value_manual")]
    public string value_manual { get; set; }
}