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

        // 1) Базовая фильтрация состава
        var filteredStaff = staff
            .Where(e => !string.Equals(e.position, "ГЕНЕРАЛЬНЫЙ ДИРЕКТОР", StringComparison.InvariantCultureIgnoreCase))
            .ToList();

        // 2) Почта по user.id (НЕ по employee.id)
        //    author_id == employee.user.id
        var emailByUserId = filteredStaff
            .Select(e => new { UserId = GetUserId(e), Email = e.email })
            .Where(x => x.UserId.HasValue && !string.IsNullOrWhiteSpace(x.Email))
            .GroupBy(x => x.UserId!.Value)
            .ToDictionary(g => g.Key, g => g.Select(x => x.Email!).First());

        // 3) Письма руководителям (оставляем как есть — по должности)
        var managersEmails = filteredStaff
            .Where(e => !string.IsNullOrWhiteSpace(e.position)
                        && ManagerPositions.Contains(e.position!.Trim())
                        && !string.IsNullOrWhiteSpace(e.email))
            .Select(e => e.email!)
            .Distinct(StringComparer.InvariantCultureIgnoreCase)
            .ToList();

        // 4) Обход всех типов списков; ownerId трактуем как author_id (= user.id)
        foreach (ListType listType in Enum.GetValues(typeof(ListType)))
        {
            var ownersWithErrors = GetOwnersWithErrors(listType); // множество author_id
            var ownersWithSuccess = ownersWithErrors
                .Union(emailByUserId.Keys) // все, кому потенциально можем писать
                .ToHashSet();

            foreach (var ownerUserId in ownersWithSuccess)
            {
                var bodyErr = BuildOwnerErrorsHtml(listType, ownerUserId);
                var bodyOk  = BuildOwnerSuccessHtml(listType, ownerUserId);

                if (IsManagerByUserId(ownerUserId, filteredStaff))
                {
                    if (string.IsNullOrWhiteSpace(bodyErr) && string.IsNullOrWhiteSpace(bodyOk))
                        continue;
                }
                else
                {
                    if (!HasOwnerAccessByUserId(ownerUserId, filteredStaff))
                        continue;
                }

                if (!emailByUserId.TryGetValue(ownerUserId, out var toEmail))
                    continue;

                var body = new StringBuilder();
                if (!string.IsNullOrWhiteSpace(bodyErr)) body.AppendLine(bodyErr);
                if (!string.IsNullOrWhiteSpace(bodyOk))  body.AppendLine(bodyOk);
                if (body.Length == 0) continue;

                var subject = $"{listType}";
                await sender.SendAsync(new[] { toEmail }, subject, body.ToString());
            }

            // 5) Сводки руководителям
            if (managersEmails.Count > 0)
            {
                var allErr = BuildAllErrorsHtml(listType);
                var allOk  = BuildAllSuccessHtml(listType);

                if (!string.IsNullOrWhiteSpace(allErr) || !string.IsNullOrWhiteSpace(allOk))
                {
                    var body = new StringBuilder();
                    if (!string.IsNullOrWhiteSpace(allErr)) body.AppendLine(allErr);
                    if (!string.IsNullOrWhiteSpace(allOk))  body.AppendLine(allOk);

                    var subject = $"{listType}";
                    await sender.SendAsync(managersEmails, subject, body.ToString());
                }
            }

            Clear(listType);
        }
    }

    /// <summary>
    /// Безопасно получаем user.id из ответа сотрудника.
    /// author_id == этот id
    /// </summary>
    private static int? GetUserId(EmployerDataResponse e)
        => e?.user?.id;

    private static bool IsManagerByUserId(int ownerUserId, List<EmployerDataResponse> staff)
    {
        var employee = staff.FirstOrDefault(e => GetUserId(e) == ownerUserId);
        return employee != null && ManagerPositions.Contains(employee.position?.Trim() ?? "");
    }

    private static bool HasOwnerAccessByUserId(int ownerUserId, List<EmployerDataResponse> staff)
    {
        // Здесь можно расширить реальной моделью прав.
        // Пока — есть сотрудник с таким user.id => доступ есть.
        var employee = staff.FirstOrDefault(e => GetUserId(e) == ownerUserId);
        return employee != null;
    }
}
