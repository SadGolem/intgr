using AutoMapper;
using integration.Context.MT;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Interfaces;
using integration.Services.Location;
using integration.Services.Location.fromMT.Storage;
using Microsoft.Extensions.Options;

public class LocationFromMTSetterService : ServiceSetterBase<LocationMTDataResponse>,
    ISetterService<LocationMTDataResponse>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILocationMTStatusStorageService _storage;
    private readonly ILogger<LocationFromMTSetterService> _logger;
    private readonly IAuth _apiSettings;
    private readonly string _endpointSetStatus;
    private readonly IMapper _mapper;
    private readonly IStatusTransitionPlanner _planner;
    private readonly ILocationStatusReader _statusReader;

    private List<LocationData> _locations = new();

    public LocationFromMTSetterService(
        IHttpClientFactory httpClientFactory,
        ILogger<LocationFromMTSetterService> logger,
        IAuthorizer authorizer,
        IOptions<AuthSettings> apiSettings,
        IMapper mapper,
        ILocationMTStatusStorageService storage,
        IStatusTransitionPlanner planner,
        ILocationStatusReader statusReader) 
        : base(httpClientFactory, logger, authorizer, apiSettings)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _apiSettings = apiSettings.Value.APROconnect;
        _endpointSetStatus = _apiSettings.BaseUrl + _apiSettings.ApiClientSettings.LocationSETStatusFromMT;
        _mapper = mapper;
        _storage = storage;
        _planner = planner;
        _statusReader = statusReader;
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
        foreach (var loc in _locations)
        {
            // Целевой статус приходит из АПРО (string)
            var targetStatusName = NormalizeTarget(loc.status);

            // Текущий статус: пробуем взять id из МТ
            int? currentId = null;
            try
            {
                currentId = await _statusReader.GetCurrentStatusIdAsync(loc.id.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Location {Id}: failed to read current status_id, will proceed without it", loc.id);
            }

            // Если у вас где-то есть name текущего статуса — подставьте:
            var currentName = loc.current_status; // или string.Empty, если нет

            var transitions = _planner.BuildPath(currentName, currentId, targetStatusName);
            if (transitions.Count == 0)
            {
                _logger.LogInformation("Location {Id}: no transitions to apply (currentId={CurId}, target='{Target}')",
                    loc.id, currentId, targetStatusName);
                continue;
            }

            foreach (var transitionId in transitions)
            {
                var requestBody = new { transition = new { id = transitionId } };
                try
                {
                    await Patch(_httpClientFactory, $"{_endpointSetStatus}{loc.id}/", requestBody, true);
                    _logger.LogInformation("Location {Id}: applied transition {TransitionId}", loc.id, transitionId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Location {Id}: failed on transition {TransitionId}, stop chain", loc.id, transitionId);
                    break; // останавливаем цепочку для данной площадки
                }
            }
        }
    }

    private static string NormalizeTarget(string status)
    {
        var s = (status ?? "").Trim();
        // «Неизвестная» трактуем как «Не проверено»
        if (string.Equals(s, "Неизвестная", StringComparison.OrdinalIgnoreCase))
            return "Не проверено";
        return s;
    }
}
