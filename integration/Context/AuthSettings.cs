using System.Globalization;

namespace integration.Context
{
    public class AuthSettings
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string CallbackUrl { get; set; }
    }

    public class ConnectngStringApro
    {
        private readonly IConfiguration _configuration;
        private string _aproConnectSettings;

        public ConnectngStringApro(IConfiguration configuration)
        {
            _configuration = configuration;
            InitializeAproConnectSettings();
        }

        private void InitializeAproConnectSettings()
        {
            var authSettings = _configuration.GetSection("APROconnect").Get<AuthSettings>();
            if (authSettings == null || string.IsNullOrEmpty(authSettings.CallbackUrl))
            {
                _aproConnectSettings = null; // Или выбросить исключение, если  это критическая ошибка
                return;
            }
            string callbackUrl = authSettings.CallbackUrl;

            // Получаем текущее время в UTC
            DateTimeOffset nowUtc = DateTimeOffset.UtcNow;

            // Вычисляем дату 3 дня назад от текущего времени (начало дня)
            DateTimeOffset threeDaysAgoUtc = nowUtc.AddDays(-3).Date; // 

            // Форматируем даты в нужный формат (yyyy-MM-dd)
            string startDate = threeDaysAgoUtc.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            string endDate = nowUtc.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);


            _aproConnectSettings = callbackUrl
                .Replace("token-auth/", $"wf__wastetakeoutrequest__garbage_collection_request/?query={{id, datetime_create, datetime_update,waste_site{{id}},client_contact{{id,name}}, author{{name}},status,volume,date, capacity{{capacity}},type{{id,name}},ext_id, comment, containers{{id}}}}&datetime_update__gte={startDate}&datetime_update_It={endDate}");


        }
        public string GetAproConnectSettings()
        {
            return _aproConnectSettings;
        }
    }
}
