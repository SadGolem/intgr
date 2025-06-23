namespace integration.Context.Request;

public class LocationRequest
{
    public int? idAsuPro { get; set; }
    public float longitude { get; set; }
    public float latitude { get; set; }
    public string? status { get; set; }
    public string? address { get; set; }
}