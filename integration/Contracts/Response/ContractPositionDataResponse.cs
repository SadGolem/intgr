using System.Text.Json.Serialization;
using integration.Context;

public class ContractPositionDataResponse : DataResponse
{
    [JsonPropertyName("id")]
    public int id { get; set; }
    
    [JsonPropertyName("number")]
    public string number { get; set; }  // Исправлено: должно быть "number" вместо "name"
    
    [JsonPropertyName("status")]
    public Status status { get; set; }

    [JsonPropertyName("waste_source")]
    public EmitterDataResponse waste_source { get; set; }

    [JsonPropertyName("waste_site")]
    public LocationDataResponse waste_site { get; set; }

    [JsonPropertyName("value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? value { get; set; }  // Изменен тип на double?

    [JsonPropertyName("value_manual")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? value_manual { get; set; }  // Изменен тип на double?

    [JsonPropertyName("contract")]
    public ContractDataResponse contract { get; set; }

    [JsonPropertyName("estimation_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? estimation_value { get; set; }  

    [JsonPropertyName("date_start")]
    public string date_start { get; set; }  

    [JsonPropertyName("date_end")]
    public string date_end { get; set; } 
}