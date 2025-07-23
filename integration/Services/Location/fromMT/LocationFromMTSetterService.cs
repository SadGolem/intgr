using AutoMapper;
using integration.Context.MT;
using integration.Context.Request.MT;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Interfaces;
using integration.Services.Location.fromMT.Storage;
using Microsoft.Extensions.Options;

namespace integration.Services.Location;

public class LocationFromMTSetterService : ServiceSetterBase<LocationMTDataResponse>,
    ISetterService<LocationMTDataResponse>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILocationMTStatusStorageService _storage;
    private readonly ILogger<LocationFromMTSetterService> _logger;
    private readonly IAuth _apiSettings;
    private readonly string _endpointSetStatus;
    private readonly IMapper _mapper;
    private List<LocationMTDataResponse> _locations = new List<LocationMTDataResponse>();

    public LocationFromMTSetterService(IHttpClientFactory httpClientFactory,
        ILogger<LocationFromMTSetterService> logger,
        IAuthorizer authorizer,
        IOptions<AuthSettings> apiSettings,
        IMapper mapper,
        ILocationMTStatusStorageService storage) : base(httpClientFactory, logger,
        authorizer, apiSettings)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _apiSettings = apiSettings.Value.APROconnect;
        _endpointSetStatus = _apiSettings.BaseUrl + _apiSettings.ApiClientSettings.LocationSETStatusFromMT;
        _mapper = mapper;
        _storage = storage;
    }

    public async Task Set()
    {
        await GetLocation();
        await SetStatus();
    }

    private async Task GetLocation()
    {
        _locations = _storage.Get();
    }

    private async Task SetStatus()
    {
        foreach (var loc in _locations.FirstOrDefault().Data)
        {
            var responce = _mapper.Map<LocationData, LocationMTStatusRequest>(loc);

            var requestBody = new
            {
                transition = new
                {
                    id = responce.status_id
                }
            };

            await Patch(
                _httpClientFactory,
                $"{_endpointSetStatus}{loc.id}/",
                requestBody,
                true
            );
        }
    }
}