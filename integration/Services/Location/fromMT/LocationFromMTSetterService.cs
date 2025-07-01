using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AutoMapper;
using integration.Context;
using integration.Context.MT;
using integration.Context.Request.MT;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Interfaces;
using integration.Services.Location.fromMT.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace integration.Services.Location.fromMT;

public class LocationFromMTSetterService : ServiceSetterBase<LocationMTDataResponse>, ISetterService<LocationMTDataResponse>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILocationMTStorageService _storage;
    private readonly ILogger<LocationFromMTSetterService> _logger;
    private readonly IAuth _apiSettings;
    private readonly string _endpointSetPhoto;
    private readonly string _endpointSetPhotoToLocation;
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
        _endpointSetPhoto = _apiSettings.BaseUrl + _apiSettings.ApiClientSettings.SetPhoto;
        _endpointSetPhotoToLocation = _apiSettings.BaseUrl + _apiSettings.ApiClientSettings.LocationSetPhoto;
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

    private async Task UploadFile(LocationMTDataResponse loc)
    {
        try
        {
            using var content = new MultipartFormDataContent();

            for (int i = 0; i < loc.images.Count; i++)
            {
                var imageBytes = loc.images[i].ToArray();
                var imageContent = new ByteArrayContent(imageBytes);
                imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
                content.Add(imageContent, $"photos", $"photo_{i}.jpg");
            }

            var requestBody = new
            {
                file = content
            };

            var response = await Post(
                _httpClientFactory,
                $"{_endpointSetPhoto}/",
                requestBody,
                true
            );

            SetFileToLocation(loc.idAPRO, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error uploading photos for location {loc.idMT}");
        }
    }

    private async Task SetFileToLocation(int loc, ActionResult<string> response)
    {
        var jsonResponse = JsonSerializer.Deserialize<JsonElement>(response.Value);
        
        long fileId = jsonResponse.GetProperty("id").GetInt64();
        
        var patchUrl = $"{_endpointSetPhotoToLocation}{loc}/";
        var patchBody = new { uploaded_files = new[] { fileId } };
            
        using var patchContent = new StringContent(
            JsonSerializer.Serialize(patchBody),
            Encoding.UTF8,
            "application/json"
        );
        
        using var patchResponse = await _httpClientFactory.CreateClient()
            .PatchAsync(patchUrl, patchContent);

        patchResponse.EnsureSuccessStatusCode();
    }
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