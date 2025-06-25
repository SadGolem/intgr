namespace integration.Context.Request;

public class LocationRequest
{
    public int? idAsuPro { get; set; }
    public decimal longitude { get; set; }
    public decimal latitude { get; set; }
    public string? status { get; set; }
    public string? address { get; set; }
}