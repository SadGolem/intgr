using System.Text.Json.Serialization;

namespace integration.Context
{
    public class ClientData : Data
    {
        public new string nameFileTime = "contragent";

        [JsonPropertyName("id")]
        public int id_oob { get; set; }
        [JsonPropertyName("name")]
        public string name { get; set; }
        [JsonPropertyName("short_name")]
        public string short_name { get; set; }
        [JsonPropertyName("inn")]
        public string inn { get; set; }
        [JsonPropertyName("kpp")]
        public string kpp { get; set; }
        [JsonPropertyName("ogrn")]
        public string ogrn { get; set; }
        [JsonPropertyName("root_company")]
        public string root_company { get; set; }
       /* [JsonPropertyName("cheif")]
        public Boss boss { get; set; }*/
        [JsonPropertyName("waste_person")]
        public string person_id {get; set;}

        public class Boss
        {
            [JsonPropertyName("id")]
            public int id { get; set; }
        }
    }
}
