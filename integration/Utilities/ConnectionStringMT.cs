using System.Globalization;
using integration.HelpClasses;
using integration.Helpers.Auth;

namespace integration.Helpers;

public class ConnectionStringMT
{
    private readonly IConfiguration _configuration;
    private string _mtConnectSettings;

    public ConnectionStringMT(IConfiguration configuration, string url)
    {
        _configuration = configuration;
        InitializeAproConnectSettings(url);
    }

    public string ReplaceStringUrl(string old, string newString)
    {
        return _mtConnectSettings.Replace(old, newString);
    }

    public string ReplaceStringUtlWithoutDate(string old, string newString)
    {
        string replaced = _mtConnectSettings.Replace(old, newString); // Заменяем old на newString
        int index = replaced.IndexOf(newString); // Находим индекс вхождения newString
        if (index != -1)
        {
            return replaced.Substring(0, index + newString.Length); // Обрезаем строку после newString
        }
        else
        {
            return replaced; // Если newString не найдена, возвращаем исходную строку после замены
        }
    }


    private void InitializeAproConnectSettings(string url)
    {
        var authSettings = _configuration.GetSection("MTconnect").Get<AuthSettings>();
        string callbackUrl = authSettings.MTconnect.CallbackUrl;
        

        // Получаем текущее время в UTC
        DateTimeOffset nowUtc = DateTimeOffset.UtcNow;

        // Вычисляем дату 3 дня назад от текущего времени (начало дня)
        DateTimeOffset threeDaysAgoUtc = nowUtc.AddDays(-3).Date; // 

        // Форматируем даты в нужный формат (yyyy-MM-dd)
        string startDate = threeDaysAgoUtc.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        string endDate = nowUtc.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);


        _mtConnectSettings = callbackUrl
            .Replace("token-auth/", $"{url}&datetime_update__gte={startDate}&datetime_update_It={endDate}");
    }

    public string GetAproConnectSettings()
    {
        return _mtConnectSettings;
    }
}