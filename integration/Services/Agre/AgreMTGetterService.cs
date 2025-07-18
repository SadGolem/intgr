using System.Data;
using integration.Context;
using integration.Context.MT;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Agre.Storage;
using integration.Services.Client.Storage;
using integration.Services.ContractPosition.Storage;
using integration.Services.Emitter;
using integration.Services.Emitter.Storage;
using integration.Services.Interfaces;
using integration.Services.Location;
using Microsoft.Extensions.Options;

namespace integration.Services.Agre;

public class AgreMTGetterService: ServiceGetterBase<AgreMTDataResponse>,
    IGetterService<AgreMTDataResponse>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AgreMTGetterService> _logger;
    private readonly string _apiSettings;
    private readonly string _connectionStringGetLocation;
    private readonly IAgreStorageService _storage;
    private List<AgreMTDataResponse> _agreList = new List<AgreMTDataResponse>();
    
    public AgreMTGetterService(IHttpClientFactory httpClientFactory, 
        ILogger<AgreMTGetterService> logger,
        IAuthorizer authorizer,
        IOptions<AuthSettings> apiSettings,
        IAgreStorageService storage) : base(httpClientFactory, logger, authorizer, apiSettings)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _apiSettings = apiSettings.Value.MTconnect.BaseUrl +
                       apiSettings.Value.MTconnect.ApiClientSettings.AgreEndpointGet;
        _storage = storage;
        _connectionStringGetLocation = apiSettings.Value.APROconnect.BaseUrl 
                                       + apiSettings.Value.APROconnect.ApiClientSettings.LocationGetEndpoint;
    }
    public async Task Get()
    {
        await GetAgreFromMT();
    }

    private async Task GetAgreFromMT()
    {
        try
        {
            _agreList = await Get(_apiSettings, false);
            await SetToListAgre(_agreList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Agre getter service failed");
            throw;
        }
    }

    private async Task SetToListAgre(List<AgreMTDataResponse> list)
    {
        foreach (var agreMt in _agreList)
        {
            try
            {

            }
            catch (

            {
                Exception
            } e)
            {
                Console.WriteLine(e);
                throw;
            }
            foreach (var agre in agreMt.Data)
            {
                _storage.Set((agre, Convert.ToInt32(agre.idLocation)));
            }
        }
    }
}