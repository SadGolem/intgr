using integration.Context;
using integration.Factory.GET.Interfaces;
using integration.Services.ContractPosition;
using integration.Services.ContractPosition.Storage;
using integration.Services.Interfaces;
using integration.Services.Location;
using integration.Services.Schedule;
using integration.Services.Storage;

namespace integration.Factory.GET;

public class ScheduleGetterServiceFactory : IGetterServiceFactory<ScheduleData>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ScheduleGetterService> _logger;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly ILocationIdService _locationIdService;
    private readonly IScheduleStorageService _scheduleStorageService;
    private readonly IContractPositionStorageService _contractPositionStorageService;
    

    public ScheduleGetterServiceFactory(
        IHttpClientFactory httpClientFactory,
        ILogger<ScheduleGetterService> logger,
        IConfiguration configuration, ILocationIdService locationIdService, IScheduleStorageService scheduleStorageService, IContractPositionStorageService contractPositionStorageService)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _configuration = configuration;
        _httpClient = new HttpClient();
        _locationIdService = locationIdService;
        _scheduleStorageService = scheduleStorageService;
        _contractPositionStorageService = contractPositionStorageService;
    }

    public IGetterService<ScheduleData> Create()
    {
        return new ScheduleGetterService(_httpClientFactory, _httpClient, _logger, _configuration, _scheduleStorageService, _contractPositionStorageService);
    }
}