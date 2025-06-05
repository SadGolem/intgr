using integration.Context;
using System.Text.Json.Serialization;

public class EntryDataResponse: DataResponse
{
    [JsonPropertyName("id")]
    public int BtNumber { get; set; }
    [JsonPropertyName("date")]
    public string? PlanDateRO { get; set; } 
    [JsonPropertyName("author")]
    public AuthorData? Author { get; set; }
    [JsonPropertyName("waste_site")]
    public LocationDataResponse location { get; set; }
    [JsonPropertyName("status_id")]
    public int status { get; set; } 
    [JsonPropertyName("capacity")]
    public Capacity Capacity { get; set; }

    
}
public class AuthorData
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
public class Capacity
{
    [JsonPropertyName("capacity")] // Ожидает свойство "capacity" внутри объекта
    public float? volume { get; set; }
}


