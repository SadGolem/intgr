using System.Text.Json.Serialization;

namespace integration.Context
{
    public class LocationData
    {
        [JsonPropertyName("id")]
        public int id { get; set; }
        [JsonPropertyName("status")]
        public int status { get; set; }
        [JsonPropertyName("datetime_create")]
        public string datetime_create { get; set; }
        [JsonPropertyName("datetime_update")]
        public string datetime_update { get; set; }
        [JsonPropertyName("lon")]
        public double lon { get; set; }
        [JsonPropertyName("lat")]
        public double lat { get; set; }
        [JsonPropertyName("address")]
        public string address { get; set; }
    }
}
