using static integration.Controllers.LocationController;
using System.Text.Json.Serialization;

namespace integration.Context
{
    public class LocationDataResponse
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }
        [JsonPropertyName("next")]
        public string Next { get; set; }
        [JsonPropertyName("previous")]
        public string Previous { get; set; }
        [JsonPropertyName("results")]
        public List<LocationData> Results { get; set; }
    }
}
