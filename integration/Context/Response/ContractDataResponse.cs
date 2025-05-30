using System.Text.Json.Serialization;

namespace integration.Context;

public class ContractDataResponse : DataResponse
{
    [JsonPropertyName("id")] //id договора (возможно допника)
    public int id { get; set; }
    [JsonPropertyName("name")]
    public string name { get; set; }
    [JsonPropertyName("status")]
    public Status status { get; set; }
    [JsonPropertyName("root_id")]
    public string root_id { get; set; }
    [JsonPropertyName("participant")]
    public ClientDataResponse client { get; set; }
    [JsonPropertyName("contract_type")]
    public ContractType contractType { get; set; }

    public class ContractType
    {
        [JsonPropertyName("name")]
        public string name { get; set; }
    }
}