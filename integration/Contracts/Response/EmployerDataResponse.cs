using System.Text.Json.Serialization;

namespace integration.Context.Response;

public class EmployerDataResponse
{
    [JsonPropertyName("id")]
    public int id { get; set; }
    [JsonPropertyName("name")]
    public string? name { get; set; } 
    [JsonPropertyName("email")]
    public string? email { get; set; }
    [JsonPropertyName("position")]
    public string? position { get; set; }
}