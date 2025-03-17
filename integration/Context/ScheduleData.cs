using System.Text.Json.Serialization;

namespace integration.Context
{
    public class ScheduleData : Data
    {
        public new string nameFileTime = "schedule";

        [JsonPropertyName("id")]
        public int id_oob { get; set; }
        [JsonPropertyName("waste_site")]
        public Location location { get; set; }
        [JsonPropertyName("containers")]
        public List<Container>? containers { get; set; }
        [JsonPropertyName("schedule")]
        public string gr_w { get; set; }
        [JsonPropertyName("dates")]
        public string[] dates { get; set; }
    }
}

