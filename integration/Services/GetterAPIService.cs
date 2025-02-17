using Microsoft.AspNetCore.Mvc;
using integration.Services.Interfaces;
using integration.Context;
using System.Net.Http;
using System.Text.Json;
using integration.Controllers;
using integration.HelpClasses;
using Microsoft.Extensions.Configuration;
using System;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using static EmailMessageBuilder;
using integration.Services.Factory.Interfaces;

namespace integration.Services
{
    public class GetterAPIService : IGetterService<Data>
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<GetterAPIService> _logger; // Correct logger type
        private readonly IConfiguration _configuration;
        private readonly IGetterServiceFactory<Data> _getterServiceFactory;
        private readonly string _mtConnect;
        private readonly string _aproConnect;

        public GetterAPIService( // Make constructor protected for inheritance
            IHttpClientFactory httpClientFactory,
            ILogger<GetterAPIService> logger,
            IConfiguration configuration,
            IGetterServiceFactory<Data> getterServiceFactory,
            string mtConnect,
            string aproConnect)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _configuration = configuration;
            _getterServiceFactory = getterServiceFactory;
            _mtConnect = mtConnect;
            _aproConnect = aproConnect;
            
        }

        public async Task<List<Data>> GetSync()
        {
            try
            {
                return await FetchData(); 
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error in GetSync"); 
                throw; 
            }
        }

        public async Task<List<Data>> FetchData() 
        {
            var data = new List<Data>();

            try
            {
                using var httpClient = _httpClientFactory.CreateClient(); 
                await Authorize(httpClient);
                var response = await httpClient.GetAsync(_aproConnect);

                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                data = await JsonSerializer.DeserializeAsync<List<Data>>(
                   await response.Content.ReadAsStreamAsync(), options);
                Message("Got: " + content);

                return data;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"Error during GET request to {_aproConnect}");
                throw;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, $"Error during JSON deserialization of response from {_aproConnect}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error while fetching data from {_aproConnect}");
                throw;
            }
        }

        public async Task Authorize(HttpClient httpClient)
        {
            var token = await GetToken(); 
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        private async Task<string> GetToken()
        {
            string token = await TokenController._authorizer.GetCachedTokenAPRO();
            return token;
        }

        public void Message(string ex)
        {
            EmailMessageBuilder.PutInformation(ListType.getlocation, ex);
        }

    }
}
