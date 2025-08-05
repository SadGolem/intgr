namespace integration.Services.CheckUp.Services;

public class BaseCheckUpService
{
    public void Message(string ex, EmailMessageBuilder.ListType type)
    {
        EmailMessageBuilder.PutInformation(type, ex);
    }
}