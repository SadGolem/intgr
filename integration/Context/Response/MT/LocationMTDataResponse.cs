using System.Text.Json.Serialization;

namespace integration.Context.MT;

public class LocationMTDataResponse : DataResponse
{
    [JsonPropertyName("idAsuPro")]
    public int idAPRO { get; set; }
    [JsonPropertyName("id")]
    public int idMT { get; set; }
    [JsonPropertyName("status")]
    public int status { get; set; }
    [JsonPropertyName("updateDate")]
    public int datetime_update { get; set; }
    [JsonPropertyName("imageBytesList")]
    public List<byte[]> images { get; set; }
}