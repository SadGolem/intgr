using System.Text.Json.Serialization;

namespace integration.Context
{
    public class LocationDataResponse : DataResponse
    {
        [JsonPropertyName("id")] public int id { get; set; }
        [JsonPropertyName("status")] public Status status { get; set; }
        [JsonPropertyName("lon")] public decimal lon { get; set; }
        [JsonPropertyName("lat")] public decimal lat { get; set; }
        [JsonPropertyName("address")] public string address { get; set; }
        [JsonPropertyName("ext_id_2")] public string? ext_id { get; set; }
        [JsonPropertyName("comment")] public string? comment { get; set; }
        
        [JsonIgnore] // Игнорируем при сериализации
        public bool IsNew => !string.IsNullOrEmpty(ext_id) && 
                             int.TryParse(ext_id, out int parsed) && 
                             parsed == 0;
        [JsonPropertyName("client_entity")] public Participant? participant { get; set; }
        [JsonPropertyName("participant")] public Participant? client { get; set; }
        public string? author_update { get; set; }
        public class Participant()
        {
            [JsonPropertyName("id")] public int id { get; set; }
            [JsonPropertyName("name")] public string name { get; set; }
        }
    }
}
