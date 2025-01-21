using System.Text.Json.Serialization;

namespace integration.Context
{
    public class WasteData
    {
        [JsonPropertyName("id")]
        public int idBT { get; set; }
        [JsonPropertyName("datetime_create")]
        public DateTime datetime_create { get; set; }
        [JsonPropertyName("datetime_update")]
        public DateTime datetime_update { get; set; }
        [JsonPropertyName("date")]
        public DateTime date { get; set; }
        [JsonPropertyName("volume")]
        public float volume { get; set; }
        [JsonPropertyName("assignee")]
        public string creator { get; set; }
        [JsonPropertyName("Status")]//такой же айди?
        public string statusID { get; set; }
        [JsonPropertyName("waste_site")]
        public float idLocation { get; set; }
        [JsonPropertyName("type")] //такой же айди?
        public int idContainerType { get; set; }
        [JsonPropertyName("number")] //такой же айди?
        public int amount { get; set; }
        [JsonPropertyName("comment")] //такой же айди?
        public int commentByRO { get; set; }
    }
}
