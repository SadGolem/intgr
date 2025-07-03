using integration.Context;
using integration.Services.CheckUp;
using integration.Services.Client.Storage;
using integration.Services.ContractPosition.Storage;
using integration.Services.Emitter.Storage;
using integration.Services.Integration;
using integration.Services.Schedule;
using integration.Services.Storage.Interfaces;
using integration.Structs;

namespace integration.Services.Storage;

public interface IConverterToStorageService
{
    //IntegrationStruct Mapping(List<ContractPositionDataResponse> context);
    Task ToStorage();
}

public class ConverterToStorageService : IConverterToStorageService
{
    private List<int> contractPositionList;
    private IScheduleStorageService _scheduleStorage;
    private IContractStorageService _contractStorageService;
    private IClientStorageService _clientStorageService;
    private IStorageService<IntegrationStruct> _storageService;
    private IContractPositionStorageService _contractPositionStorageService;
    private IEmitterStorageService _emitterStorageService;
    private readonly IntegrationStructValidator _validator;
    private List<ContractDataResponse> contracts = new List<ContractDataResponse>();
    private List<ScheduleDataResponse> schedules = new List<ScheduleDataResponse>();
    private List<EmitterDataResponse> emitters = new List<EmitterDataResponse>();
    private List<ClientDataResponse> clients = new List<ClientDataResponse>();
    private List<ContractPositionDataResponse> contractPositions = new List<ContractPositionDataResponse>();
    
    public ConverterToStorageService(IScheduleStorageService scheduleStorage, IContractStorageService contractStorageService,
        IClientStorageService clientStorageService, IContractPositionStorageService contractPositionStorageService,
        IEmitterStorageService emitterStorageService, IStorageService<IntegrationStruct> storageService,
        IntegrationStructValidator validator)
    {
        _scheduleStorage = scheduleStorage;
        _contractStorageService = contractStorageService;
        _clientStorageService = clientStorageService;
        _contractPositionStorageService = contractPositionStorageService;
        _emitterStorageService = emitterStorageService; 
        _storageService = storageService;
        _validator = validator;
    }

    public async Task GetAll()
    {
        contractPositions = _contractPositionStorageService.Get();
        contracts = _contractStorageService.Get();
        schedules = _scheduleStorage.Get();
        clients = _clientStorageService.Get();
        emitters = _emitterStorageService.Get();
    }
    
    public async Task ToStorage()
    {
        await GetAll();
        await CreateStruct();
    }

    private async Task CreateStruct()
    {
        foreach (var position in contractPositions)
        {
            var integrationStruct = CreateStruct(position);
            
            if (!_validator.Validate(integrationStruct).Result)
            {
                continue;
            }

            _storageService.Set(integrationStruct);
        }
    }

    private IntegrationStruct CreateStruct(ContractPositionDataResponse context)
    {
        int idLocation = context.waste_site.id;
        List<ContractDataResponse> contractDatas = new List<ContractDataResponse>();
        List<EmitterDataResponse> emitterDatas = new List<EmitterDataResponse>();
        List<ClientDataResponse> clientDatas = new List<ClientDataResponse>();
        List<ScheduleDataResponse> scheduleDatas = new List<ScheduleDataResponse>();
        LocationDataResponse locationDatasResponse = new LocationDataResponse();

        int idPos = context.id;
        contractDatas = contracts;
        emitterDatas = emitters;
        clientDatas = clients;
        locationDatasResponse = context.waste_site;
        foreach (var schedule in scheduleDatas)
        {
            if (locationDatasResponse.id == schedule.location.id)
            {
                scheduleDatas.Add(schedule);
                break;
            }
        }

        foreach (var contract in contractDatas)
        {
            if (context.contract.root_id == contract.root_id)
            {
                contractDatas.Add(contract);
                break;
            }
        }

        
    return new IntegrationStruct(idLocation, emitterDatas, clientDatas, scheduleDatas, contractDatas, locationDatasResponse);
    }
}