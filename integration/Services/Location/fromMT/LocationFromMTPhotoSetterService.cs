using System.Net.Http.Headers;
using System.Text.Json;
using AutoMapper;
using integration.Context.MT;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Interfaces;
using integration.Services.Location.fromMT.Storage;
using Microsoft.Extensions.Options;

namespace integration.Services.Location.fromMT;

public class LocationFromMTPhotoSetterService : ServiceSetterBase<LocationMTPhotoDataResponse>, ISetterService<LocationMTPhotoDataResponse>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILocationMTStorageService _storage;
    private readonly ILogger<LocationFromMTPhotoSetterService> _logger;
    private readonly IAuth _apiSettings;
    private readonly string _endpointSetPhoto;
    private readonly string _endpointSetPhotoToLocation;
    private readonly IMapper _mapper;
    private List<LocationMTPhotoDataResponse> _locations;

    public LocationFromMTPhotoSetterService(IHttpClientFactory httpClientFactory,
        ILogger<LocationFromMTPhotoSetterService> logger,
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
        _mapper = mapper;
        _storage = storage;
    }

    public async Task Set()
    {
        await GetLocation();
        
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
                
                string fileName = $"saved-{DateTime.Now:yyyyMMdd_HHmmss}_p{loc.idAPRO}.jpg";
            
                var imageBytes = image.ToArray();
                var imageContent = new ByteArrayContent(imageBytes);
                
                imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
                imageContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "file",
                    FileName = fileName
                };
                
                content.Add(imageContent);
                
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
    private async Task SetFileToLocation(int loc, string response)
    {
        var jsonResponse = JsonSerializer.Deserialize<JsonElement>(response);
        
        long fileId = jsonResponse.GetProperty("id").GetInt64();
        
        var patchUrl = $"{_endpointSetPhotoToLocation}{loc}/";
        var patchBody = new { uploaded_files = new[] { fileId } };
        
        await Patch(_httpClientFactory, patchUrl, patchBody, true);
    }
}
