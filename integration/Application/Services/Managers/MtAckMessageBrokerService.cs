using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using integration.Context.Request;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Location;
using Microsoft.Extensions.Options;

namespace integration.Application.Services.Managers;

public class MtAckMessageBrokerService :  ServiceSetterBase<AgreMTResponse>, IMessageBroker
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IAuthorizer _authorizer;
    private readonly ILogger<MtAckMessageBrokerService> _logger;
    private readonly string _ackEndpoint;

    public MtAckMessageBrokerService(
        IHttpClientFactory httpClientFactory,
        ILogger<MtAckMessageBrokerService> logger,
        IAuthorizer authorizer,
        IOptions<AuthSettings> apiSettings)  : base(httpClientFactory, logger, authorizer, apiSettings)
    {
        _httpClientFactory = httpClientFactory;
        _authorizer = authorizer;
        _logger = logger;
        
        _ackEndpoint = apiSettings.Value.MTconnect.BaseUrl +
                       apiSettings.Value.MTconnect.ApiClientSettings.AgreAckEndpoint;
    }

    public async Task<AgreMTResponse> PublishAckAsync(IEnumerable<string> sourceKeys, CancellationToken ct = default)
    {
        var payloadJson = JsonSerializer.Serialize(sourceKeys);
        

        using var content = new StringContent(payloadJson, Encoding.UTF8, "application/json"); 
        var response = await Post(_httpClientFactory, _ackEndpoint, content, false);
        var body = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("ACK publish failed {StatusCode}. Body: {Body}", response.StatusCode, body);
            response.EnsureSuccessStatusCode();
        }

        var ack = JsonSerializer.Deserialize<AgreMTResponse>(body, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? throw new InvalidOperationException("ACK response is null");

        _logger.LogInformation("ACK accepted by MT: message='{Message}', timestamp={Timestamp:o}, data={Data}",
            ack.Message, ack.Timestamp, ack.Data);

        return ack;
    }
}
