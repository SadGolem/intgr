using System.Net;
using System.Text;
using System.Text.Json;
using integration.Context;
using integration.HelpClasses;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace integration.Services.Location;

public class LocationSetterService : ServiceBase, ISetterService<LocationData>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<LocationSetterService> _logger;
    private readonly IAuth _apiSettings;
    private readonly ILocationMapper _mapper;
    private readonly ILocationValidator _validator;

    public LocationSetterService(
        IHttpClientFactory httpClientFactory,
        ILogger<LocationSetterService> logger,
        IAuthorizer authorizer,
        IOptions<AuthSettings> apiSettings,
        ILocationMapper mapper,
        ILocationValidator validator)
        : base(httpClientFactory,logger, authorizer, apiSettings)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _apiSettings = apiSettings.Value.MTconnect;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task PostAndPatch(IEnumerable<(LocationData Data, bool IsNew)> locations)
    {
        using var client = await Authorize(false);

        var tasks = locations.Select(l => 
            ProcessLocationAsync(client, l.Data, l.IsNew));
        
        await Task.WhenAll(tasks);
    }

    private async Task ProcessLocationAsync(HttpClient client, LocationData data, bool isNew)
    {
        try
        {
            if (!_validator.Validate(data))
            {
                _logger.LogWarning("Validation failed for location {LocationId}", data.id);
                return;
            }

            var endpoint = isNew 
                ? _apiSettings.ApiClientSettings.CreateLocationEndpoint 
                : $"{_apiSettings.ApiClientSettings.UpdateLocationEndpoint}/{data.id}";

            var response = await SendRequestAsync(
                client,
                endpoint,
                _mapper.MapToRequest(data),
                isNew ? HttpMethod.Post : HttpMethod.Patch);

            await HandleResponseAsync(response, data.id, isNew);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing location {LocationId}", data.id);
            throw new IntegrationException($"Location {data.id} processing failed", ex);
        }
    }

    private async Task<HttpResponseMessage> SendRequestAsync(
        HttpClient client,
        string endpoint,
        object data,
        HttpMethod method)
    {
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var request = new HttpRequestMessage(method, endpoint)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        return await client.SendAsync(request);
    }

    private async Task HandleResponseAsync(HttpResponseMessage response, int locationId, bool isNew)
    {
        var action = isNew ? "created" : "updated";
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to {Action} location {LocationId}. Status: {StatusCode}, Response: {Response}",
                action, locationId, response.StatusCode, content);
            throw new ApiIntegrationException(response.StatusCode, content);
        }

        _logger.LogInformation("Successfully {Action} location {LocationId}. Response: {Response}",
            action, locationId, content);
    }
    
    public Task PostAndPatch(List<(LocationData, bool)> data)
    {
        throw new NotImplementedException();
    }
}

// Дополнительные классы
public interface ILocationMapper
{
    LocationRequest MapToRequest(LocationData location);
}

public class LocationMapper : ILocationMapper
{
    public LocationRequest MapToRequest(LocationData location) => new(
        IdBT: location.id,
        Longitude: (decimal)location.lon,
        Latitude: (decimal)location.lat,
        Status: StatusCoder.ToCorrectLocationStatus(location.status),
        Address: location.address
    );
}

public interface ILocationValidator
{
    bool Validate(LocationData location);
}

public class LocationValidator : ILocationValidator
{
    public bool Validate(LocationData location)
    {
        if (string.IsNullOrWhiteSpace(location.address))
        {
            throw new ValidationException("Address is required");
        }
        return true;
    }
}

public record LocationRequest(
    int IdBT,
    decimal Longitude,
    decimal Latitude,
    string Status,
    string Address);

// Исключения
public class IntegrationException : Exception
{
    public IntegrationException(string message, Exception inner) 
        : base(message, inner) { }
}

public class ApiIntegrationException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public string Response { get; }

    public ApiIntegrationException(HttpStatusCode statusCode, string response)
        : base($"API request failed with status {statusCode}")
    {
        StatusCode = statusCode;
        Response = response;
    }
}

public class AuthorizationException : Exception
{
    public AuthorizationException(string message, Exception inner)
        : base(message, inner) { }
}

public class ValidationException : Exception
{
    public ValidationException(string message)
        : base(message) { }
}