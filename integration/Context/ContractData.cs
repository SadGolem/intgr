using System.Text.Json.Serialization;

namespace integration.Context;

public partial class ContractData : Data
{
    [JsonPropertyName("id")] //id договора (возможно допника)
    public int id { get; set; }
    [JsonPropertyName("number")]
    public string number { get; set; }
    [JsonPropertyName("status")]
    public Status status { get; set; }
    [JsonPropertyName("root_id")]
    public string root_id { get; set; }
    [JsonPropertyName("client_id")]
    public ClientData client { get; set; }
}