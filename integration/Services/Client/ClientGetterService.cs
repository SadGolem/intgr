using integration.Context;
using integration.HelpClasses;
using integration.Helpers;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Client.Storage;
using integration.Services.ContractPosition.Storage;
using integration.Services.Interfaces;
using integration.Services.Location;
using Microsoft.Extensions.Options;

namespace integration.Services.Client;

public class ClientGetterService : ServiceGetterBase<ClientData>, IGetterService<ClientData>
{
   private readonly IContractPositionStorageService _positionStorage;
    private readonly IClientStorageService _clientStorage;
    private readonly APROconnectSettings _apiSettings;
    private readonly ILogger<ClientGetterService> _logger;
    private IHttpClientFactory _httpClientFactory;

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
            _clientStorage.SetClients(clients);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Client synchronization failed");
            throw;
        }
        
        
    }

    private async Task<IEnumerable<ClientIdentifier>> GetClientIdentifiers()
    {
        var positions = _positionStorage.GetPosition();
        return positions.Select(p => new ClientIdentifier(
            p.contract.client.idAsuPro,
            p.contract.client.doc_type.name
        )).Distinct();
    }

    private async Task<List<ClientData>> ProcessClientsAsync(IEnumerable<ClientIdentifier> clientIds)
    {
        var clients = new List<ClientData>();
        
        foreach (var clientId in clientIds)
        {
            var client = await FetchClientDataAsync(clientId);
            if (client != null)
            {
                clients.Add(client);
            }
        }
        
        return clients;
    }

    private async Task<ClientData> FetchClientDataAsync(ClientIdentifier clientId)
    {
        try
        {
            var endpoint = BuildClientEndpoint(clientId);
            var response = await Get(_httpClientFactory, endpoint);
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
        var basePath = clientId.IsLegalEntity 
            ? _apiSettings.ApiClientSettings.LegalEntitiesEndpoint
            : _apiSettings.ApiClientSettings.IndividualsEndpoint;
            
        return $"{basePath}{clientId.Id}";
    }

    private async Task EnrichClientDataAsync(List<ClientData> clients)
    {
        var tasks = new List<Task>
        {
            EnrichWithBankDetailsAsync(clients),
            EnrichWithContactDetailsAsync(clients)
        };

        await Task.WhenAll(tasks);
    }

    private async Task EnrichWithBankDetailsAsync(List<ClientData> clients)
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
            var endpoint = $"{_apiSettings.ApiClientSettings.BankDetailsEndpoint}{clientId}";
            var response = await Get(_httpClientFactory, endpoint);
            return response.FirstOrDefault()?.bik;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to fetch BIK for client {ClientId}", clientId);
            return null;
        }
    }

    private async Task EnrichWithContactDetailsAsync(List<ClientData> clients)
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
            var endpoint = $"{_apiSettings.ApiClientSettings.ContactsEndpoint}{clientId}";
            var response = await Get(_httpClientFactory, endpoint);
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