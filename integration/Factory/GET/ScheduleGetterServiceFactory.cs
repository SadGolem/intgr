using integration.Context;
using integration.Factory.GET.Interfaces;
using integration.HelpClasses;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.ContractPosition;
using integration.Services.ContractPosition.Storage;
using integration.Services.Interfaces;
using integration.Services.Location;
using integration.Services.Schedule;
using integration.Services.Storage;
using Microsoft.Extensions.Options;

namespace integration.Factory.GET;

public class ScheduleGetterServiceFactory : IGetterServiceFactory<ScheduleData>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ScheduleGetterService> _logger;
    private readonly IOptions<AuthSettings>  _configuration;
    private readonly IScheduleStorageService _scheduleStorageService;
    private readonly IContractPositionStorageService _contractPositionStorageService;
    private readonly IAuthorizer _authorizer;
    

    public ScheduleGetterServiceFactory(
        IHttpClientFactory httpClientFactory,
        ILogger<ScheduleGetterService> logger,
        IAuthorizer authorizer,
        IOptions<AuthSettings> configuration, IScheduleStorageService scheduleStorageService, IContractPositionStorageService contractPositionStorageService)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _authorizer = authorizer;
        _configuration = configuration;
        _scheduleStorageService = scheduleStorageService;
        _contractPositionStorageService = contractPositionStorageService;
    }

    public IGetterService<ScheduleData> Create()
    {
        return new ScheduleGetterService(_httpClientFactory, _logger, _authorizer, _configuration, _scheduleStorageService, _contractPositionStorageService);
    }
}