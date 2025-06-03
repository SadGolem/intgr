using System.Text.Json.Serialization;

namespace integration.Context
{
    public class EmitterDataResponse : DataResponse
    {
        public new string nameFileTime = "emitter";

        [JsonPropertyName("id")]
        public int id { get; set; }
        [JsonPropertyName("external_id")]
        public int ext_id { get; set; }
        [JsonPropertyName("name")]
        public string name { get; set; }
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
        public string amount { get; set; }
        public string contractNumber { get; set; }
        public int? location_mt_id { get; set; }
        public string executorName { get; set; }
        public int idContract { get; set; }
        public string contractStatus { get; set; }
        public string typeConsumer { get; set; }
        [JsonPropertyName("waste_source_categoty")]
        public EmitterCategory emitterCategory { get; set; }

        public class EmitterCategory
        {
            [JsonPropertyName("name")] 
            public string name { get; set; } // Добавлен public
        }
    }
}
