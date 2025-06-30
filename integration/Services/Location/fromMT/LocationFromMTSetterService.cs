using System.Net.Http.Headers;
using AutoMapper;
using integration.Context;
using integration.Context.MT;
using integration.Context.Request.MT;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Interfaces;
using integration.Services.Location.fromMT.Storage;
using Microsoft.Extensions.Options;

namespace integration.Services.Location.fromMT;

public class LocationFromMTSetterService : ServiceSetterBase<LocationMTDataResponse>, ISetterService<LocationMTDataResponse>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILocationMTStorageService _storage;
    private readonly ILogger<LocationFromMTSetterService> _logger;
    private readonly IAuth _apiSettings;
    private readonly string _endpointSetPhoto;
    private readonly string _endpointSetStatus;
    private readonly IMapper _mapper;
    private List<LocationMTDataResponse> _locations;

    public LocationFromMTSetterService(IHttpClientFactory httpClientFactory,
        ILogger<LocationFromMTSetterService> logger,
        IAuthorizer authorizer,
        IOptions<AuthSettings> apiSettings,
        IMapper mapper,
        ILocationMTStorageService storage) : base(httpClientFactory, logger,
        authorizer, apiSettings)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _apiSettings = apiSettings.Value.APROconnect;
        _endpointSetPhoto = _apiSettings.BaseUrl + _apiSettings.ApiClientSettings.LocationSetPhoto;
        _endpointSetStatus = _apiSettings.BaseUrl + _apiSettings.ApiClientSettings.LocationSETStatusFromMT;
        _mapper = mapper;
        _storage = storage;
    }

    public async Task Set()
    {
        await GetLocation();
        await SetStatus();
        await SetPhoto();
    }

    private async Task GetLocation()
    {
        _locations = _storage.Get();
    }
    private async Task SetPhoto()
    {
        foreach (var loc in _locations)
        {
            // Пропускаем локации без фотографий
            if (loc.images == null || loc.images.Count == 0)
                continue;

            try
            {
                // Создаем multipart-контент
                using var content = new MultipartFormDataContent();
            
                // Добавляем ID локации
                content.Add(new StringContent(loc.idMT.ToString()), "id");
            
                // Добавляем статус
                content.Add(new StringContent(loc.status.ToString()), "status_id");
            
                // Добавляем все фотографии
                for (int i = 0; i < loc.images.Count; i++)
                {
                    var imageBytes = loc.images[i].ToArray();
                    var imageContent = new ByteArrayContent(imageBytes);
                    imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
                    content.Add(imageContent, $"photos", $"photo_{i}.jpg");
                }

                // Отправляем запрос
                await Post(
                    _httpClientFactory,
                    $"{_endpointSetPhoto}/",
                    content, 
                    true
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error uploading photos for location {loc.idMT}");
            }
        }
    }
    
    private acync 

    private async Task SetStatus()
    {
        foreach (var loc in _locations)
        {
            var responce = _mapper.Map<LocationMTDataResponse, LocationMTRequest>(loc);

            var requestBody = new
            {
                transition = new
                {
                    id = responce.status_id
                }
            };

            await Patch(
                _httpClientFactory,
                $"{_endpointSetStatus}{loc.idAPRO}/",
                requestBody,
                true
            );
        }
    }
}