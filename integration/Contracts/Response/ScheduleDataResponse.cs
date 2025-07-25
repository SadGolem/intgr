using System.Text.Json.Serialization;

namespace integration.Context
{
    public class ScheduleDataResponse : DataResponse
    {
        public new string nameFileTime = "schedule";

        [JsonPropertyName("id")]
        public int id { get; set; }
        [JsonPropertyName("waste_site")]
        public Location location { get; set; }
        [JsonPropertyName("containers")]
        public List<Container>? containers { get; set; }
        [JsonPropertyName("schedule")]
        public string gr_w { get; set; }
        [JsonPropertyName("dates")]
        public string[] dates { get; set; }

        public string? ext_id;
        public EmitterDataResponse? emitter { get; set; }
        public LocationDataResponse? LocationDataResponse { get; set; }
        
        public int? idContainerType { get; set; }
    }
}

