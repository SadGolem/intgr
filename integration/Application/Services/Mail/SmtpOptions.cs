namespace integration.Application.Services.Mail;

public sealed class SmtpOptions
{
    public string Host { get; init; } = "smtp.mail.ru";
    public int Port { get; init; } = 587;
    public string Username { get; init; } = "";
    public string Password { get; init; } = "";
    public bool EnableSsl { get; init; } = true;
}
