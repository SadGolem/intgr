using System.Text.Json.Serialization;

namespace integration.Context
{
    public class EmitterDataResponse : DataResponse
    {
        public new string nameFileTime = "emitter";

        [JsonPropertyName("waste_source")]
        public WasteSource WasteSource { get; set; }  // Новый класс-обёртка
        [JsonPropertyName("containers")]
        public List<Container>? container { get; set; }
        [JsonPropertyName("external_id")]
        public string ext_id { get; set; }
        public string amount { get; set; } //объем
        public string contractNumber { get; set; }
        public string location_mt_id { get; set; }
        public string executorName { get; set; }
        public int idContract { get; set; }
        public string contractStatus { get; set; }
        public int participant_id { get; set; }
        public string typeConsumer { get; set; }
        public string nameConsumer { get; set; }
        public string idConsumer { get; set; }
    }

    public class WasteSource {
        [JsonPropertyName("id")]
        public int id { get; set; }

        [JsonPropertyName("address")]
        public string address { get; set; }

        [JsonPropertyName("name")]
        public string name { get; set; }

        [JsonPropertyName("waste_source_category")]  // Теперь на правильном уровне
        public WasteSourceCategory category { get; set; }

        [JsonPropertyName("normative_unit_value_exist")]
        public bool normative { get; set; }
    }

    public class WasteSourceCategory {
        [JsonPropertyName("id")] 
        public int id { get; set; }

        [JsonPropertyName("name")]
        public string name { get; set; }
    }
}
