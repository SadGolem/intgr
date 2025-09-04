using integration.Context.Response;
using Microsoft.Extensions.Options;

namespace integration.Application.Services.Mail;

using System.Net;
using System.Net.Mail;
using System.Text;
using static EmailMessageBuilder;
using integration.Services.Employers.Storage;

public interface IEmailSender
{
    Task SendAsync(IEnumerable<string> toEmails, string subject, string bodyHtml);
}

public class SmtpEmailSender : IEmailSender
{
    private readonly SmtpOptions _opt;
    private readonly SmtpClient _client;
    private readonly string _from;
    public SmtpEmailSender(IOptions<SmtpOptions> opt)
    {
        _opt = opt.Value;
        _from = _opt.Username;
        _client = new SmtpClient(_opt.Host, _opt.Port)
        {
            Credentials = new NetworkCredential(_opt.Username, _opt.Password),
            EnableSsl = _opt.EnableSsl
        };
    }
    
    /*public SmtpEmailSender(string host, int port, string username, string password, bool enableSsl = true)
    {
        _from = username;
       
    }*/

    public async Task SendAsync(IEnumerable<string> toEmails, string subject, string bodyHtml)
    {
        foreach (var to in toEmails.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct())
        {
            using var mail = new MailMessage
            {
                From = new MailAddress(_from),
                Subject = subject,
                Body = bodyHtml,
                IsBodyHtml = true
            };
            mail.To.Add(to);

            try { await _client.SendMailAsync(mail); }
            catch (Exception ex) { Console.WriteLine($"Ошибка отправки на {to}: {ex.Message}"); }
        }
    }
}

public static class EmailDispatcher
{
    private static readonly HashSet<string> ManagerPositions =
        new(StringComparer.InvariantCultureIgnoreCase)
        {
            "Главный специалист", "Начальник отдела"
        };

    public static async Task DispatchAsync(IEmployersStorageService employers, IEmailSender sender)
    {
        var staff = employers.Get() ?? new List<EmployerDataResponse>();
        
        var filteredStaff = staff
            .Where(e => e.position != "ГЕНЕРАЛЬНЫЙ ДИРЕКТОР")
            .ToList();

        var emailById = filteredStaff
            .Where(e => e.id > 0 && !string.IsNullOrWhiteSpace(e.email))
            .GroupBy(e => e.id)
            .ToDictionary(g => g.Key, g => g.First().email!);

        // Получаем список email руководителей
        var managersEmails = filteredStaff
            .Where(e => !string.IsNullOrWhiteSpace(e.position) &&
                        ManagerPositions.Contains(e.position!.Trim()) &&
                        !string.IsNullOrWhiteSpace(e.email))
            .Select(e => e.email!)
            .Distinct()
            .ToList();

        foreach (ListType listType in Enum.GetValues(typeof(ListType)))
        {
            var ownersWithErrors = GetOwnersWithErrors(listType);
            var ownersWithSuccess = GetOwnersWithErrors(listType) 
                .Union(emailById.Keys) 
                .ToHashSet();
            
            foreach (var ownerId in ownersWithSuccess)
            {
                var bodyErr = BuildOwnerErrorsHtml(listType, ownerId);
                var bodyOk = BuildOwnerSuccessHtml(listType, ownerId);
                
                if (IsManager(ownerId, filteredStaff))
                {
                    if (string.IsNullOrWhiteSpace(bodyErr) && string.IsNullOrWhiteSpace(bodyOk))
                        continue;
                }
                else
                {
                    if (!HasOwnerAccess(ownerId, filteredStaff))
                        continue;
                }

                if (!emailById.TryGetValue(ownerId, out var toEmail))
                    continue;

                var body = new StringBuilder();
                if (!string.IsNullOrWhiteSpace(bodyErr)) body.AppendLine(bodyErr);
                if (!string.IsNullOrWhiteSpace(bodyOk)) body.AppendLine(bodyOk);
                
                if (body.Length == 0)
                    continue;

                var subject = $"{listType}";
                await sender.SendAsync(new[] { toEmail }, subject, body.ToString());
            }
            
            if (managersEmails.Count > 0)
            {
                var allErr = BuildAllErrorsHtml(listType);
                var allOk = BuildAllSuccessHtml(listType);
                
                if (string.IsNullOrWhiteSpace(allErr) && string.IsNullOrWhiteSpace(allOk))
                    continue;

                var body = new StringBuilder();
                if (!string.IsNullOrWhiteSpace(allErr)) body.AppendLine(allErr);
                if (!string.IsNullOrWhiteSpace(allOk)) body.AppendLine(allOk);

                var subject = $"{listType}";
                await sender.SendAsync(managersEmails, subject, body.ToString());
            }

            Clear(listType);
        }
    }
    
    private static bool IsManager(int ownerId, List<EmployerDataResponse> staff)
    {
        var employee = staff.FirstOrDefault(e => e.id == ownerId);
        return employee != null && ManagerPositions.Contains(employee.position?.Trim());
    }
    
    private static bool HasOwnerAccess(int ownerId, List<EmployerDataResponse> staff)
    {
        var employee = staff.FirstOrDefault(e => e.id == ownerId);
        return employee != null && employee.id == ownerId; // author_update_id == ownerId
    }

}
