using System.Text.Json.Serialization;

namespace integration.Context.MT;

public class EntryMTDataResponse : DataResponse
{
    [JsonPropertyName("id")]
    public int idAPRO { get; set; }
    [JsonPropertyName("status")]
    public string? status { get; set; } 
}