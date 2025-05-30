namespace integration.Context.Request;

public class EmitterRequest
{
    public int idAsuPro { get; set; }
    public int idConsumer { get; set; }
    public int idConsumerType { get; set; }
    public int amount { get; set; }
    public string consumerAddress { get; set; }
    public string accountingType { get; set; } //норматив
    public string contractNumber { get; set; } 
    public int idLocation { get; set; } 
    public string executorName { get; set; } 
    public int idContract { get; set; } 
    public string contractStatus { get; set; } 
    public string addressBT { get; set; }
    public string usernameBT { get; set; }

}