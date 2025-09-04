namespace integration.Services.CheckUp.Services;

public class BaseCheckUpService
{
    // Ошибка (старый метод оставлен ради обратной совместимости)
    public void Message(string ex, EmailMessageBuilder.ListType type)
        => Message(ex, type, null);

    public void Message(string ex, EmailMessageBuilder.ListType type, int? authorUpdateId)
        => EmailMessageBuilder.PutError(type, ex, authorUpdateId);

    // Успех по площадке
    public void Success(EmailMessageBuilder.ListType type, int locationId, int? authorUpdateId, string? extra = null)
        => EmailMessageBuilder.PutSuccess(type, locationId, authorUpdateId, extra);
}
