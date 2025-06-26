using System.Text.Json.Serialization;

namespace integration.Context.MT;

public class EntryMTDataResponse : DataResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("data")]
    public List<EntryData> Data { get; set; }
}
public class EntryData
{
    [JsonPropertyName("idAsuPro")]
    public int id { get; set; }  // Единый стиль именования

    [JsonPropertyName("executeStatus")]
    public string status { get; set; } = string.Empty;  // Инициализация по умолчанию
}