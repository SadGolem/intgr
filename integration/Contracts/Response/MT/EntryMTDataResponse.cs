using System.Text.Json.Serialization;
using integration.Context;

public class EntryMTDataResponse : DataResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("data")]
    public List<EntryData>? Data { get; set; }
}

public class EntryData
{
    [JsonPropertyName("idAsuPro")]
    public int id { get; set; }

    [JsonPropertyName("executeStatus")]
    public string status { get; set; } = string.Empty;
    [JsonPropertyName("modified")]
    public DateTime timestamp { get; set; }
    
    [JsonPropertyName("volf")]
    public decimal fact { get; set; }
    [JsonPropertyName("kolf")]
    public int countContainer { get; set; }
}