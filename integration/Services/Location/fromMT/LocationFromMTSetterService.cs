using System.Net.Http.Headers;
using System.Text.Json;
using AutoMapper;
using integration.Context.MT;
using integration.Context.Request.MT;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Interfaces;
using integration.Services.Location.fromMT.Storage;
using Microsoft.Extensions.Options;

namespace integration.Services.Location.fromMT;

public class LocationFromMTSetterService : ServiceSetterBase<LocationMTPhotoDataResponse>, ISetterService<LocationMTPhotoDataResponse>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILocationMTStorageService _storage;
    private readonly ILogger<LocationFromMTSetterService> _logger;
    private readonly IAuth _apiSettings;
    private readonly string _endpointSetPhoto;
    private readonly string _endpointSetPhotoToLocation;
    private readonly string _endpointSetStatus;
    private readonly IMapper _mapper;
    private List<LocationMTPhotoDataResponse> _locations;

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
        _endpointSetPhoto = _apiSettings.BaseUrl + _apiSettings.ApiClientSettings.SetPhoto;
        _endpointSetPhotoToLocation = _apiSettings.BaseUrl + _apiSettings.ApiClientSettings.LocationGetEndpoint;
        _endpointSetStatus = _apiSettings.BaseUrl + _apiSettings.ApiClientSettings.LocationSETStatusFromMT;
        _mapper = mapper;
        _storage = storage;
    }

    public async Task Set()
    {
        await GetLocation();
            // await SetStatus();
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
            if (loc.images == null || loc.images.Count == 0)
                continue;
            await UploadFile(loc);
        }
    }

    private async Task UploadFile(LocationMTPhotoDataResponse loc)
    {
        foreach (var image in loc.images)
        {
            try
            {
                using var content = new MultipartFormDataContent();
            
                // Генерируем уникальное имя файла как в примере
                string fileName = $"saved-{DateTime.Now:yyyyMMdd_HHmmss}_p{loc.idAPRO}.jpg";
            
                var imageBytes = image.ToArray();
                var imageContent = new ByteArrayContent(imageBytes);
            
                // Устанавливаем заголовки как в примере
                imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
                imageContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "file",
                    FileName = fileName
                };
            
                // Добавляем файл в контент
                content.Add(imageContent);

                // Отправляем запрос
                var response = await PostPhoto(
                    _httpClientFactory,
                    _endpointSetPhoto,
                    content,
                    true
                );

                SetFileToLocation(loc.idAPRO, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error uploading file for location {loc.idAPRO}");
            }
        }
    }
    private async Task DebugSaveMultipart(MultipartFormDataContent content, string path)
    {
        using var stream = new MemoryStream();
        await content.CopyToAsync(stream);
        File.WriteAllBytes(path, stream.ToArray());
    }
    private async Task SetFileToLocation(int loc, string response)
    {
        var jsonResponse = JsonSerializer.Deserialize<JsonElement>(response);
        
        long fileId = jsonResponse.GetProperty("id").GetInt64();
        
        var patchUrl = $"{_endpointSetPhotoToLocation}{loc}/";
        var patchBody = new { uploaded_files = new[] { fileId } };
        
        await Patch(_httpClientFactory, patchUrl, patchBody, true);
        
    }
    private async Task SetStatus()
    {
        foreach (var loc in _locations)
        {
            var responce = _mapper.Map<LocationMTPhotoDataResponse, LocationMTPhotoRequest>(loc);

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