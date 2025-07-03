using integration;
using integration.Context;
using integration.Controllers;
using integration.Controllers.Apro;
using integration.Factory;
using integration.Factory.GET;
using integration.Factory.GET.Interfaces;
using integration.Factory.SET;
using integration.Factory.SET.Interfaces;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.CheckUp;
using integration.Services.CheckUp.Factory;
using integration.Services.Client;
using integration.Services.Client.Storage;
using integration.Services.ContractPosition;
using integration.Services.ContractPosition.Storage;
using integration.Services.Integration;
using integration.Services.Interfaces;
using integration.Services.Location;
using integration.Services.Schedule;
using integration.Services.Storage;
using integration.Services.CheckUp.Services;
using integration.Services.Integration.Interfaces;
using integration.Services.Integration.Processors;
using integration.Services.Token;
using integration.Services.Token.Interfaces;
using AutoMapper;
using integration.Context.MT;
using integration.Context.Request.MT;
using integration.Factory.GET.MT;
using integration.Services.Emitter;
using integration.Services.Emitter.Storage;
using integration.Services.Entry;
using integration.Services.Entry.MT;
using integration.Services.Entry.MT.Storage;
using integration.Services.Entry.Storage;
using integration.Services.Location.fromMT;
using integration.Services.Location.fromMT.Storage;
using integration.Services.Storage.Interfaces;
using integration.Structs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();

builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection("AuthSettings"));
builder.Services.Configure<ApiClientSettings>(builder.Configuration.GetSection("APROconnect:ApiClientSettings"));

builder.Services.AddScoped<IApiClientService, ApiClientService>();
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddSingleton<IAuthorizer, Authorizer>();
builder.Services.AddSingleton<ILocationIdService, LocationIdService>();
builder.Services.AddSingleton<IEntryStorageService<EntryDataResponse>, EntryStorageService>();
builder.Services.AddSingleton<IEntryStorageService<EntryMTDataResponse>, EntryMTStorageService>();
builder.Services.AddSingleton<ILocationMTStorageService, LocationMTStorageService>();
builder.Services.AddSingleton<IScheduleStorageService, ScheduleStorageService>();
builder.Services.AddSingleton<IContractPositionStorageService, ContractPositionStorageService>();
builder.Services.AddSingleton<IClientStorageService, ClientStorageService>();
builder.Services.AddSingleton<IContractStorageService, ContractStorageService>();
//builder.Services.AddSingleton<IIntegrationStructStorageService, StorageService>();
builder.Services.AddSingleton<IStorageService<IntegrationStruct>, StorageService>();
builder.Services.AddSingleton<IConverterToStorageService, ConverterToStorageService>();
builder.Services.AddSingleton<IEmitterStorageService, EmitterStorageService>();
builder.Services.AddTransient<IGetterServiceFactory<DataResponse>, DataGetterServiceFactory>(); 
builder.Services.AddTransient<IGetterLocationServiceFactory<LocationDataResponse>, LocationGetterServiceFactory>();
builder.Services.AddTransient<IGetterLocationService<LocationDataResponse>, LocationGetterService>();
builder.Services.AddTransient<IGetterServiceFactory<LocationMTDataResponse>, LocationMTGetterServiceFactory>();
builder.Services.AddTransient<IGetterService<LocationMTDataResponse>, LocationMTGetterService>();
builder.Services.AddTransient<ISetterServiceFactory<LocationDataResponse>, LocationSetterServiceFactory>();
builder.Services.AddTransient<ISetterService<LocationDataResponse>, LocationSetterService>();
builder.Services.AddTransient<ISetterServiceFactory<EntryDataResponse>, EntrySetterServiceFactory>();
builder.Services.AddTransient<IGetterServiceFactory<EntryMTDataResponse>, EntryMTGetterServiceFactory>();
builder.Services.AddTransient<ISetterService<EntryDataResponse>, EntrySetterService>();
builder.Services.AddTransient<ISetterServiceFactory<EntryMTRequest>, EntryFromMTServiceFactory>();
builder.Services.AddTransient<ISetterService<EntryMTRequest>, EntryFromMTSetterService>();
builder.Services.AddTransient<ISetterServiceFactory<LocationMTDataResponse>, LocationFromMTSetterServiceFactory>();
builder.Services.AddTransient<ISetterService<LocationMTDataResponse>, LocationFromMTSetterService>();
builder.Services.AddScoped<ILocationValidator, LocationValidator>();
builder.Services.AddTransient<IGetterServiceFactory<ScheduleDataResponse>, ScheduleGetterServiceFactory>();
builder.Services.AddTransient<IGetterService<ScheduleDataResponse>, ScheduleGetterService>();
builder.Services.AddTransient<IGetterServiceFactory<ContractPositionDataResponse>, ContractPositionGetterServiceFactory>();
builder.Services.AddTransient<IGetterService<ContractPositionDataResponse>, ContractPositionGetterService>();
builder.Services.AddTransient<IGetterServiceFactory<ContractDataResponse>, ContractGetterServiceFactory>();
builder.Services.AddTransient<IGetterService<ContractDataResponse>, ContractGetterService>();
builder.Services.AddTransient<IGetterServiceFactory<ClientDataResponse>, ClientGetterServiceFactory>();
builder.Services.AddTransient<IGetterService<ClientDataResponse>, ClientGetterService>();
builder.Services.AddTransient<IGetterServiceFactory<EmitterDataResponse>, EmitterGetterServiceFactory>();
builder.Services.AddTransient<IGetterService<EmitterDataResponse>, EmitterGetterService>();
builder.Services.AddTransient<IGetterServiceFactory<EntryDataResponse>, EntryGetterServiceFactory>();
builder.Services.AddTransient<IGetterService<EntryDataResponse>, EntryGetterService>();
builder.Services.AddTransient<IGetterServiceFactory<LocationMTDataResponse>, LocationMTGetterServiceFactory>();
builder.Services.AddTransient<IGetterService<LocationMTDataResponse>, LocationMTGetterService>();
builder.Services.AddScoped<IClientCheckUpService, ClientCheckUpService>();
builder.Services.AddScoped<IEmitterCheckUpService, EmitterCheckUpService>();
builder.Services.AddScoped<ILocationCheckUpService, LocationCheckUpService>();
builder.Services.AddScoped<IScheduleCheckUpService, ScheduleCheckUpService>();

builder.Services.AddScoped<IntegrationStructValidator>();
builder.Services.AddScoped<IIntegrationProcessor<ClientDataResponse>, ContragentProcessor>();
builder.Services.AddScoped<IIntegrationProcessor<EmitterDataResponse>, EmitterProcessor>();
builder.Services.AddScoped<IIntegrationProcessor<LocationDataResponse>, LocationProcessor>();
builder.Services.AddScoped<IIntegrationProcessor<ScheduleDataResponse>, ScheduleProcessor>();
builder.Services.AddScoped<IAproClientService, AproClientService>();

builder.Services.AddScoped<IApiClientService, ApiClientService>();
builder.Services.AddScoped<IIntegrationService, IntegrationService>();
builder.Services.AddScoped<ICheckUpFactory<ClientDataResponse>, ClientCheckUpFactory>();
builder.Services.AddScoped<ICheckUpService<ClientDataResponse>, ClientCheckUpService>();
builder.Services.AddScoped<ICheckUpFactory<EmitterDataResponse>, EmitterCheckUpFactory>();
builder.Services.AddScoped<ICheckUpService<EmitterDataResponse>, EmitterCheckUpService>();
builder.Services.AddScoped<ICheckUpFactory<ScheduleDataResponse>, ScheduleCheckUpFactory>();
builder.Services.AddScoped<ICheckUpService<ScheduleDataResponse>, ScheduleCheckUpService>();
builder.Services.AddScoped<ICheckUpFactory<LocationDataResponse>, LocationCheckUpFactory>();
builder.Services.AddScoped<ICheckUpService<LocationDataResponse>, LocationCheckUpService>();

builder.Services.AddSingleton<TokenController>();
builder.Services.AddSingleton<LocationController>();
builder.Services.AddSingleton<ScheduleController>();
builder.Services.AddSingleton<ContractPositionController>();
builder.Services.AddSingleton<ContractController>();
builder.Services.AddSingleton<ClientController>();
builder.Services.AddSingleton<EmitterController>();
builder.Services.AddSingleton<EntryController>();
builder.Services.AddSingleton<ScheduleController>();
builder.Services.AddSingleton<IntegrationController>();
builder.Services.AddHostedService<MainSyncService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer();
builder.Services.AddAuthorization();

var mapperConfig = new MapperConfiguration(cfg => 
{
    cfg.AddProfile<IntegrationMappingProfile>();
});

builder.Services.AddSingleton<IMapper>(sp => mapperConfig.CreateMapper());
var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

