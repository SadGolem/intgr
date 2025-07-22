namespace integration.Context.Request.MT;

public class LocationMTPhotoRequest
{
    public int id { get; set; }
    public int status_id { get; set; }
    public List<byte[]> photos { get; set; } 
}