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
    [JsonPropertyName("agreement")]
    public Agreement? agreement { get; set; } 
    [JsonPropertyName("comment")]
    public string? comment { get; set; } 
    [JsonPropertyName("volume")]
    public float? volume { get; set; } 
    [JsonPropertyName("capacity")]
    public Capacity? Capacity { get; set; }
    [JsonPropertyName("number")]
    public int? number { get; set; }
    public int? idContainerType { get; set; }
    public string? statusString { get; set; }

}
public class AuthorData
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
public class Capacity
{
    [JsonPropertyName("capacity")] 
    public float? volume { get; set; }
    [JsonPropertyName("id")] 
    public int? id { get; set; }
    [JsonPropertyName("type")] 
    public Types type { get; set; }
    
}
public class Agreement
{
    [JsonPropertyName("id")] 
    public int id { get; set; }
}
public class Types
{
    [JsonPropertyName("id")] 
    public int id { get; set; }
}


