using integration.Context;
using integration.HelpClasses;
using integration.Services.Client.Storage;
using integration.Services.ContractPosition.Storage;
using integration.Services.Interfaces;
using integration.Services.Location;

namespace integration.Services.Client;

public class ClientGetterService : ServiceGetterBase<ClientData>, IGetterService<ClientData>
{
    private readonly IHttpClientFactory _httpClientFactory;
 //   private IContractStorageService _contractStorageService;

    private readonly IConfiguration _configuration;

    // private IConverterToStorageService _converterToStorageService = converterToStorageService;
    private IContractPositionStorageService _contractPositionStorageService;
    private IClientStorageService _clientStorage;
    private List<int> _locationIdSList;

    private string _aproConnectUrlit =
        "wf__participant__legal_entity/?query={id,datetime_create,datetime_update,name, full_name, inn,kpp, chief{id,name}}&id=";

    private string _aproConnectUrlitURL = "";
    private string _aproConnectPhysicsURL = "";
    private string _getBIKURL = "";
    private string _getMailURL = "";
    private string _getBossURL = "";
    private readonly string _aproConnectPhysics =
        "wf__participant__fl/?query={id,datetime_create,datetime_update,name, full_name, inn,kpp, chief{id,name}}&id=";
    private readonly string _getBIK =
        "wf__account__bank_account_details/?query={bik}&participant_id=";
    private readonly string _getMail =
        "wf__contact__counterparties_contacts/?participant=5778712&query={id,contact_type{id,name},value}&contact_type_id=3";
    private readonly string _getBoss =
        "wf__employee__employee/?participant=986874&query={id,name,position}";
    
    private List<(int,string)> clients_id = new List<(int,string)>();
    List<ClientData> clients = new List<ClientData>();

    public ClientGetterService(IHttpClientFactory httpClientFactory,
        HttpClient httpClient,
        ILogger<ClientGetterService> logger,
        IConfiguration configuration,
        IContractPositionStorageService contractPositionStorageService) : base(httpClientFactory, httpClient, logger, configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _aproConnectUrlitURL = new ConnectingStringApro(configuration, _aproConnectUrlit).ReplaceStringUrlWithoutDate("&id=","&id=");
        _aproConnectPhysicsURL = new ConnectingStringApro(configuration, _aproConnectPhysics).ReplaceStringUrlWithoutDate("id=","id=");
        _getBIK = new ConnectingStringApro(configuration, _getBIK).ReplaceStringUrlWithoutDate("participant_id=","participant_id=");
        _getMailURL = new ConnectingStringApro(configuration, _getMail).ReplaceStringUrlWithoutDate("contact_type_id=3","contact_type_id=3");
        _getBossURL = new ConnectingStringApro(configuration, _getBoss).ReplaceStringUrlWithoutDate("query={id,name,position}","query={id,name,position}");
        
        _contractPositionStorageService = contractPositionStorageService;
    }
    public async Task Get()
    {
        //сначала получить uuid 
        await GetContractsToList();
        await GetClientsDataFromAPRO();
        await GetBik();
        await GetMail();
        //передать список в сторэйдж
        await ToSetClient();
        
    }

    private async Task GetContractsToList()
    {
        List<ContractPositionData> contractsPosList = _contractPositionStorageService.GetPosition();

        foreach (var con in contractsPosList)
        {
            clients_id.Add((con.contract.client.id, con.contract.client.doc_type.name));
        }
        
    }

    private async Task GetClientsDataFromAPRO()
    {
        List<ClientData> client = new List<ClientData>();
        
        foreach (var cl in clients_id)
        {
            try
            {
                if (cl.Item2 != "Юридические лица")
                    client = await Get(_httpClientFactory, _aproConnectPhysicsURL + cl.Item1);
                else
                {
                    client = await Get(_httpClientFactory, _aproConnectUrlitURL + cl.Item1);
                }

                if (client.Count > 0)
                {
                    clients.Add(client.First());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }

    private async Task ToSetClient()
    {
        foreach (var client in clients)
        {
            _clientStorage.SetClient(client);
        }
    }

    //добавление бик
    private async Task GetBik()
    {
        foreach (var cl in clients)
        {
            List<ClientData> bik = new List<ClientData>();
            try
            {
                bik = await Get(_httpClientFactory, _getBIK + cl.id);
                if (bik.Count > 0)
                    clients.ElementAt(cl.id).bik = bik.First().bik;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
    //получение электронной почты
    private async Task GetMail()
    {
        foreach (var cl in clients)
        {
            string url = new ConnectingStringApro(_configuration, _getMail).ReplaceStringUrlWithoutDate("5778712",cl.id.ToString());
            List<ClientData> mail = new List<ClientData>();
            try
            {
                mail = await Get(_httpClientFactory, url);
                if (mail.Count > 0)
                    clients.ElementAt(cl.id).mail = mail.First().mail;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}