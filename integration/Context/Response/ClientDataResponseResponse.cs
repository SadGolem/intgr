using System.Text.Json.Serialization;

namespace integration.Context
{
    public class ClientDataResponseResponse : DataResponse
    {
        public new string nameFileTime = "contragent";

        [JsonPropertyName("id")]
        public int idAsuPro { get; set; }
        [JsonPropertyName("ext_id")]
        public int? ext_id { get; set; }
        [JsonPropertyName("name")]
        public string consumerName { get; set; }
        [JsonPropertyName("bik")]
        public string bik { get; set; }
        [JsonPropertyName("value")]
        public string mailAddress { get; set; }
        [JsonPropertyName("short_name")]
        public string shortName { get; set; }
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
        [JsonPropertyName("doc_type")]
        public Doc_type doc_type { get; set; }
        
        public class Doc_type
        {
            [JsonPropertyName("name")]
            public string name { get; set; }

        }

        public class Boss
        {
            [JsonPropertyName("id")]
            public int id { get; set; }
        }

        public int GetIntegrationExtId()
        {
            if (ext_id != null)
                return ext_id.Value;
            else
            {
                return 0;
            }
        }

        public void UpdateIntegrationId(int newId)
        {
            idAsuPro = newId;
        }
    }
}
