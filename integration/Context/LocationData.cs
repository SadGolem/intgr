using System.Text.Json.Serialization;

namespace integration.Context
{
    public partial class LocationData : Data
    {
        [JsonPropertyName("id")] public int id { get; set; }
        [JsonPropertyName("status_id")] public int status { get; set; }
        [JsonPropertyName("datetime_create")] public DateTime datetime_create { get; set; }
        [JsonPropertyName("datetime_update")] public DateTime datetime_update { get; set; }
        [JsonPropertyName("lon")] public decimal lon { get; set; }
        [JsonPropertyName("lat")] public decimal lat { get; set; }
        [JsonPropertyName("address")] public string address { get; set; }
        
        [JsonPropertyName("participant")] public Participant participant { get; set; }

        public class Participant()
        {
            [JsonPropertyName("id")] public int id { get; set; }
            [JsonPropertyName("name")] public string name { get; set; }
        }
    }
}
