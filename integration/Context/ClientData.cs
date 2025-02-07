using System.Text.Json.Serialization;

namespace integration.Context
{
    public class ClientData : Data
    {
        public new string nameFileTime = "contragent";

        [JsonPropertyName("id")]
        public int id_oob { get; set; }
        [JsonPropertyName("name")]
        public int name { get; set; }
        [JsonPropertyName("short_name")]
        public Location short_name { get; set; }
        [JsonPropertyName("inn")]
        public int inn { get; set; }
        [JsonPropertyName("kpp")]
        public int kpp { get; set; }
        [JsonPropertyName("ogrn")]
        public int ogrn { get; set; }
        [JsonPropertyName("root_company")]
        public int root_company { get; set; }
        [JsonPropertyName("cheif")]
        public Boss boss { get; set; }
        [JsonPropertyName("waste_person")]
        public int person_id { get; set; }

        public class Boss
        {
            [JsonPropertyName("id")]
            public int id { get; set; }
        }
    }
}
