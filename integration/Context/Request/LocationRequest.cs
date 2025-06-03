namespace integration.Context.Request;

public class LocationRequest
{
    public int? idAsuPro { get; set; }
    public long? longitude { get; set; }
    public long? latitude { get; set; }
    public string? status { get; set; }
    public string? address { get; set; }
}