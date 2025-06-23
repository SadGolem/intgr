namespace integration.Context.Request;

public class LocationRequest
{
    public int? idAsuPro { get; set; }
    public double longitude { get; set; }
    public double latitude { get; set; }
    public string? status { get; set; }
    public string? address { get; set; }
}