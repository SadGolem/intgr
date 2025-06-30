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

public class LocationFromMTSetterService : ServiceSetterBase<LocationMTDataResponse>, ISetterService<LocationDataResponse>
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
    }

    public async Task GetLocation()
    {
        _locations = _storage.Get();
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