using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services;
using Microsoft.Extensions.Options;

public interface ILocationStatusReader
{
    Task<int?> GetCurrentStatusIdAsync(string locationId, CancellationToken ct = default);
}

/// <summary>
/// Читает текущий status_id площадки из MT:
/// GET {BaseUrlMT}/wf__waste_site__waste_site/{id}/?query={status_id}
/// </summary>
public sealed class LocationStatusReader : ServiceBase, ILocationStatusReader
{
    private readonly IAuth _apro;   // если пригодится
    private readonly IAuth _mt;
    private readonly ILogger<LocationStatusReader> _logger;

    private readonly string _mtWasteSiteEndpoint; // например: "/wf__waste_site__waste_site/"

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public LocationStatusReader(
        IHttpClientFactory httpClientFactory,
        ILogger<LocationStatusReader> logger,
        IAuthorizer authorizer,
        IOptions<AuthSettings> settings)
        : base(httpClientFactory, logger, authorizer, settings)
    {
        _logger = logger;
        _apro = settings.Value.APROconnect;
        _mt   = settings.Value.MTconnect;

        // Вынеси в конфиг, как у тебя уже сделано:
        _mtWasteSiteEndpoint = _mt.BaseUrl + _mt.ApiClientSettings.LocationGetEndpoint; 
        // например: BaseUrl = "https://mt/api", LocationGetEndpoint = "/wf__waste_site__waste_site/"
    }

    private sealed class WasteSiteStatusDto
    {
        [JsonPropertyName("status_id")]
        public int? StatusId { get; set; }
    }

    public async Task<int?> GetCurrentStatusIdAsync(string locationId, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(locationId))
        {
            _logger.LogWarning("GetCurrentStatusIdAsync: empty locationId");
            return null;
        }

        // Итоговый URL MT: .../wf__waste_site__waste_site/{id}/?query={status_id}
        // Буквальные фигурные скобки нужны по твоему контракту:
        var url = $"{_mtWasteSiteEndpoint}{locationId}/?query={{status_id}}";

        try
        {
            var client = await Authorize(isApro: false); // false => MT
            using var resp = await client.GetAsync(url, ct);

            if (resp.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Location {Id}: not found (404) when reading status_id", locationId);
                return null;
            }

            resp.EnsureSuccessStatusCode();

            await using var stream = await resp.Content.ReadAsStreamAsync(ct);
            var dto = await JsonSerializer.DeserializeAsync<WasteSiteStatusDto>(stream, JsonOpts, ct);

            if (dto?.StatusId is int sid)
                return sid;

            _logger.LogWarning("Location {Id}: status_id not found in response", locationId);
            return null;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Location {Id}: cancelled while reading status_id", locationId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Location {Id}: failed to read status_id", locationId);
            // по твоей политике: либо проглотить и вернуть null, либо пробросить
            return null;
        }
    }
}
