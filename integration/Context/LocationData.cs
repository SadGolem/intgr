﻿using System.Text.Json.Serialization;

namespace integration.Context
{
    public class LocationData
    {
        [JsonPropertyName("id")]
        public int id { get; set; }
        [JsonPropertyName("status_id")]
        public int status { get; set; }
        [JsonPropertyName("datetime_create")]
        public DateTime datetime_create { get; set; }
        [JsonPropertyName("datetime_update")]
        public DateTime datetime_update { get; set; }
        [JsonPropertyName("lon")]
        public double lon { get; set; }
        [JsonPropertyName("lat")]
        public double lat { get; set; }
        [JsonPropertyName("address")]
        public string address { get; set; }
    }
}
