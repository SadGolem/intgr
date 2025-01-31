using System;
using System.Net;
using System.Net.Mail;
using System.Collections.Generic;
using System.Threading.Tasks;
using static EmailMessageBuilder;
using System.Text;

public static class EmailSender
{
    private static string smtpHost = "smtp.mail.ru";
    private static int smtpPort = 587;
    private static string smtpUsername = "zma2002@mail.ru";
    private static string smtpPassword = "2a2BWuQae8ZYzJJTHaaw";
    private static bool enableSsl = true;

    public static async Task SendEmailsAsync(List<string> toEmails, string subject, string body)
    {
        using (var smtpClient = new SmtpClient(smtpHost, smtpPort)) // Указываем порт
        {
            smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
            smtpClient.EnableSsl = enableSsl;

            foreach (string toEmail in toEmails)
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpUsername),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(toEmail);

                try
                {
                    await smtpClient.SendMailAsync(mailMessage);
                    Console.WriteLine($"Отправлено письмо на {toEmail}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка отправки на {toEmail}: {ex.Message}");
                }
                mailMessage.Dispose();
            }
        }
    }

    private static string BuildBodyEmail(ListType listType)
    {
        return EmailMessageBuilder.GetList(listType).ToString() ?? "";
    }

    public static async Task Send()
    {
        List<string> recipients = new List<string> {
            "zubcova_ma@kuzro.ru" // Отправляем самому себе для теста
         };

        StringBuilder fullBody = new StringBuilder();
        foreach (ListType listType in Enum.GetValues(typeof(ListType)))
        {
            fullBody.Clear();
            /*fullBody.AppendLine($"<h2>{listType}</h2>");*/
            fullBody.Append("<pre>"); // Для лучшего отображения многострочных данных
            fullBody.Append(BuildBodyEmail(listType));
            fullBody.Append("</pre>");
            fullBody.AppendLine("<br>");

            string emailSubject = listType.ToString();
            string emailBody = fullBody.ToString();

            if (emailBody == "<pre></pre><br>\r\n")
                return;
            await SendEmailsAsync(recipients, emailSubject, emailBody);
        }
    }

        
}
