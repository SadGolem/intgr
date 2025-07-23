using integration.Context.MT;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Client.Storage;
using integration.Services.Interfaces;
using integration.Services.Location.fromMT.Storage;
using Microsoft.Extensions.Options;

namespace integration.Services.Location.fromMT;

public class LocationMTGetterService : ServiceGetterBase<LocationMTDataResponse>, IGetterService<LocationMTDataResponse>
{
    private readonly IClientStorageService _clientStorage;
    private readonly MTconnectSettings _apiSettings;
    private string _getEndpoint;
    private readonly ILogger<LocationMTPhotoGetterService> _logger;
    private IHttpClientFactory _httpClientFactory;
    private ILocationMTStatusStorageService _storageService;
    private const int ONE_HOUR_CONST = 60;

    public LocationMTGetterService(IHttpClientFactory httpClientFactory,
        ILogger<LocationMTPhotoGetterService> logger,
        IAuthorizer authorizer, IOptions<AuthSettings> apiSettings,
        ILocationMTStatusStorageService storageService) : base(httpClientFactory, logger, authorizer, apiSettings)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _apiSettings = apiSettings.Value.MTconnect;
        _getEndpoint = _apiSettings.BaseUrl +
                       _apiSettings.ApiClientSettings.LocationGetStatusEndpoint + "0";
        _storageService = storageService;
    }

    public async Task Get()
    {
        var locationsStatus = await Get(BuildEndpoint().Result, false);
        _storageService.Set(locationsStatus);
    }

    private async Task<string> BuildEndpoint()
    {
        DateTimeOffset startDto = new DateTimeOffset(DateTime.Now, TimeSpan.Zero);
        DateTimeOffset inPlus7 = startDto.ToOffset(TimeSpan.FromHours(7));
        
        return _getEndpoint + inPlus7.AddMinutes(-ONE_HOUR_CONST).ToString("yyyy-MM-ddTHH:mm:ss");
    }
}