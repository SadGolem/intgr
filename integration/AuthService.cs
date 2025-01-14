using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace integration
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly string _authUrl = "http://10.5.5.205:9002/auth";

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetAuthTokenAsync(string username, string password)
        {
            var authData = new
            {
                username = username,
                password = password
            };
            var json = JsonConvert.SerializeObject(authData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(_authUrl, content);

            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            // Предполагаем что ответ содержит токен в формате {"token": "ваш_токен"}
            var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseBody);

            return tokenResponse?.Token;
        }

        private class TokenResponse
        {
            [JsonProperty("token")]
            public string Token { get; set; }
        }
    }
}
