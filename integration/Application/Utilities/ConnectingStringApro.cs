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
    private void InitializeAproConnectSettings(string url)
    {
        // Получаем текущее время в UTC
        DateTimeOffset nowUtc = DateTimeOffset.UtcNow;
        
        DateTimeOffset threeDaysAgoUtc = nowUtc.AddDays(-1).Date;
        
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