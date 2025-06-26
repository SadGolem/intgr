using System.Text.Json.Serialization;

namespace integration.Context.MT;

public class EntryMTDataResponse : DataResponse
{
    [JsonPropertyName("idAsuPro")]
    public int id { get; set; }
    [JsonPropertyName("status")]
    public string? status { get; set; } 
}