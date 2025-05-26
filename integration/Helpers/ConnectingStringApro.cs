using System.Globalization;
using integration.Helpers.Auth;
using Microsoft.Extensions.Options;

namespace integration.Helpers;

public class ConnectingStringApro
{
    private readonly APROconnectSettings _aproSettings;
    private string _aproConnectSettings;

    public ConnectingStringApro(IOptions<AuthSettings> authSettings, string url)
    {
        _aproSettings = authSettings.Value.APROconnect;
        InitializeAproConnectSettings(url);
    }

    public string ReplaceStringUtl(string old, string newString)
    {
        return _aproConnectSettings.Replace(old, newString);
    }

    public string ReplaceStringUrlWithoutDate(string old, string newString)
    {
        string replaced = _aproConnectSettings.Replace(old, newString); // Заменяем old на newString
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

        

        // Получаем текущее время в UTC
        DateTimeOffset nowUtc = DateTimeOffset.UtcNow;

        // Вычисляем дату 3 дня назад от текущего времени (начало дня)
        DateTimeOffset threeDaysAgoUtc = nowUtc.AddDays(-3).Date; // 

        // Форматируем даты в нужный формат (yyyy-MM-dd)
        string startDate = threeDaysAgoUtc.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        string endDate = nowUtc.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);


        _aproConnectSettings = _aproSettings.CallbackUrl
            .Replace("token-auth/", $"{url}&datetime_update__gte={startDate}&datetime_update_It={endDate}");
    }

    public string GetAproConnectSettings()
    {
        return _aproConnectSettings;
    }
}