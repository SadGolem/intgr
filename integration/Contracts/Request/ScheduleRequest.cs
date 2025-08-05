namespace integration.Context.Request;

public class ScheduleRequest
{
    public int? idWasteGenerator { get; set; } 
    public int? idLocation { get; set; } 
    public int? amount { get; set; } 
    public int? idContainerType { get; set; } 
    public string? exportSchedule { get; set; } 
    public string? address { get; set; } //адрес контейнеройной площадки
}