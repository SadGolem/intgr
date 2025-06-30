using System.Text.Json.Serialization;

namespace integration.Context.Request;

public class PhotoApiResponse
{
    [JsonPropertyName("locationId")]
    public int LocationId { get; set; }
    
    [JsonPropertyName("images")]
    public List<string> Images { get; set; } 
}