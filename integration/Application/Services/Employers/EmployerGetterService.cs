using integration.Context.Response;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Employers.Storage;
using integration.Services.Interfaces;
using integration.Services.Location;
using Microsoft.Extensions.Options;

namespace integration.Services.Employers;

public class EmployerGetterService: ServiceGetterBase<EmployerDataResponse>, IGetterService<EmployerDataResponse>
{
    private readonly string _endpoint;
    private readonly IEmployersStorageService _storage;
    public EmployerGetterService(IHttpClientFactory httpClientFactory, 
        ILogger<ServiceBase> logger, IAuthorizer authorizer,
        IOptions<AuthSettings> apiSettings,
        IEmployersStorageService storage) : base(httpClientFactory, logger, authorizer, apiSettings)
    {
        _endpoint = apiSettings.Value.APROconnect.BaseUrl + apiSettings.Value.APROconnect.ApiClientSettings.EmployersEndpoint;
        _storage = storage;
    }

    public async Task Get()
    {
        try
        {
            var response = await Get(_endpoint, true);
            _storage.Set(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get emails");
        }
    }
}