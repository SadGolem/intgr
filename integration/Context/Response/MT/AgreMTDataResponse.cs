using System.Text.Json.Serialization;

namespace integration.Context.MT;

public class AgreMTDataResponse
{
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("data")]
    public List<AgreData>? Data { get; set; }
}

public class AgreData
{
    [JsonPropertyName("sourceKey")]
    public string idLocation { get; set; }

    [JsonPropertyName("modified")]
    public DateTime Timestamp { get; set; }

    [JsonPropertyName("comment")]
    public string comment { get; set; }

    [JsonPropertyName("username")]
    public string username { get; set; }
}