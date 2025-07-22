using System.Text.Json.Serialization;

namespace integration.Context.MT;

public class LocationMTDataResponse
{
    [JsonPropertyName("message")] public string Message { get; set; }

    [JsonPropertyName("timestamp")] public DateTime Timestamp { get; set; }

    [JsonPropertyName("count")] public int Count { get; set; }

    [JsonPropertyName("data")] public List<LocationData>? Data { get; set; }
}

public class LocationData
{
    [JsonPropertyName("idAsuPro")] public int id { get; set; }

    [JsonPropertyName("id")] public string idLocMT { get; set; } = string.Empty;

    [JsonPropertyName("modified")] public DateTime Timestamp { get; set; }

    [JsonPropertyName("status")] public string status { get; set; } = string.Empty;
}