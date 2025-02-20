using System.Text.Json.Serialization;

namespace integration.Context
{
    public class EmitterData : Data
    {
        public new string nameFileTime = "emitter";

        [JsonPropertyName("id")]
        public int id { get; set; }
        [JsonPropertyName("name")]
        public int name { get; set; }
        [JsonPropertyName("participant")]
        public ClientContact client { get; set; }
        [JsonPropertyName("address")]
        public string address { get; set; }
        [JsonPropertyName("normative_unit_value_exist")]
        public bool normative { get; set; }
        [JsonPropertyName("status")]
        public Status status { get; set; }
        [JsonPropertyName("author")]
        public Author? author { get; set; }

        [JsonPropertyName("waste_source_categoty")]
        public EmitterCategory emitterCategory { get; set; }

        public class EmitterCategory()
        {
            [JsonPropertyName("name")] string name { get; set; }
        }
    }
}
