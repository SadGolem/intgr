using System.Diagnostics.Contracts;
using integration.Context;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Client.Storage;
using integration.Services.ContractPosition.Storage;
using integration.Services.Interfaces;
using integration.Services.Location;
using Microsoft.Extensions.Options;

namespace integration.Services.Client;

public class ClientGetterService : ServiceGetterBase<ClientDataResponse>, IGetterService<ClientDataResponse>
{
    private readonly IContractPositionStorageService _positionStorage;
    private readonly IClientStorageService _clientStorage;
    private readonly APROconnectSettings _apiSettings;
    private readonly ILogger<ClientGetterService> _logger;
    private IHttpClientFactory _httpClientFactory;
    private List<ContractPositionDataResponse> _positions;

    public ClientGetterService(
        IHttpClientFactory httpClientFactory,
        ILogger<ClientGetterService> logger,
        IAuthorizer authorizer,
        IOptions<AuthSettings> apiSettings,
        IContractPositionStorageService positionStorage,
        IClientStorageService clientStorage)
        : base(httpClientFactory, logger, authorizer, apiSettings)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _apiSettings = apiSettings.Value.APROconnect;
        _positionStorage = positionStorage;
        _clientStorage = clientStorage;
    }

    public async Task Get()
    {
        try
        {
            var clientIds = await GetClientIdentifiers();
            var clients = await ProcessClientsAsync(clientIds);
            await EnrichClientDataAsync(clients);
            _clientStorage.Set(clients);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Client synchronization failed");
            throw;
        }


    }

    private async Task<IEnumerable<ClientIdentifier>> GetClientIdentifiers()
    { 
        _positions = _positionStorage.Get();
        return _positions.Select(p => new ClientIdentifier(
            p.contract.client.idAsuPro,
            p.contract.client.doc_type.name
        )).Distinct();
    }

    private async Task<List<ClientDataResponse>> ProcessClientsAsync(IEnumerable<ClientIdentifier> clientIds)
    {
        var clients = new List<ClientDataResponse>();

        foreach (var clientId in clientIds)
        {
            var client = await FetchClientDataAsync(clientId);
            if (client != null)
            {
                await GetTypeKA(client);
                clients.Add(client);
            }
        }
        return clients;
    }

    private async Task GetTypeKA(ClientDataResponse client)
    {
        foreach (var pos in _positions)
        {
            if (client.idAsuPro == pos.contract.client.idAsuPro)
            {
                if (pos.contract.contractType == null || string.IsNullOrEmpty(pos.contract.contractType.name) || 
                    string.IsNullOrEmpty(pos.contract.client.consumerName))
                {
                    //Message($"Contract id {pos.contract.id} and number {pos.contract.name} don't have a contract type.");
                    return;
                }
                
                var type = pos.contract.contractType.name;
                client.type_ka = StatusCoder.GetTypeContragent(type);
                client.doc_type = pos.contract.client.doc_type;
                client.address = pos.waste_site.address;
                
            }
        }
    }

    private async Task<ClientDataResponse> FetchClientDataAsync(ClientIdentifier clientId)
    {
        try
        {
            var endpoint = BuildClientEndpoint(clientId);
            var response = await Get(endpoint, true);
            return response.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch client {ClientId}", clientId.Id);
            return null;
        }
    }

    private string BuildClientEndpoint(ClientIdentifier clientId)
    {
        var basePath = _apiSettings.BaseUrl + (clientId.IsLegalEntity
            ? _apiSettings.ApiClientSettings.LegalEntitiesEndpoint
            : _apiSettings.ApiClientSettings.IndividualsEndpoint);

        return $"{basePath}{clientId.Id}";
    }

    private async Task EnrichClientDataAsync(List<ClientDataResponse> clients)
    {
        var tasks = new List<Task>
        {
            EnrichWithBankDetailsAsync(clients),
            EnrichWithContactDetailsAsync(clients)
        };

        await Task.WhenAll(tasks);
    }
  
    private async Task EnrichWithBankDetailsAsync(List<ClientDataResponse> clients)
    {
        foreach (var client in clients)
        {
            var bik = await FetchBankDetailsAsync(client.idAsuPro);
            client.bik = bik;
        }
    }

    private async Task<string> FetchBankDetailsAsync(int clientId)
    {
        try
        {
            var endpoint = $"{_apiSettings.BaseUrl + _apiSettings.ApiClientSettings.BankDetailsEndpoint}{clientId}";
            var response = await Get(endpoint, true);
            return response.FirstOrDefault()?.bik;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to fetch BIK for client {ClientId}", clientId);
            return null;
        }
    }

    private async Task EnrichWithContactDetailsAsync(List<ClientDataResponse> clients)
    {
        foreach (var client in clients)
        {
            var email = await FetchContactEmailAsync(client.idAsuPro);
            client.mailAddress = email;
        }
    }

    private async Task<string> FetchContactEmailAsync(int clientId)
    {
        try
        {
            var endpoint = $"{_apiSettings.BaseUrl + _apiSettings.ApiClientSettings.ContactsEndpoint}{clientId}";
            var response = await Get(endpoint, true);
            return response.FirstOrDefault()?.mailAddress;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to fetch email for client {ClientId}", clientId);
            return null;
        }
    }

    private record ClientIdentifier(int Id, string DocumentType)
    {
        public bool IsLegalEntity => DocumentType == "Юридические лица";
    }
}