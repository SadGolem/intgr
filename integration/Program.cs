using integration;
using integration.Context;
using integration.Controllers;
using integration.Controllers.Apro;
using integration.Controllers.MT;
using integration.Factory;
using integration.Factory.GET;
using integration.Factory.GET.Interfaces;
using integration.Factory.SET;
using integration.Factory.SET.Interfaces;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services;
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

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();

builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection("AuthSettings"));
builder.Services.Configure<ApiClientSettings>(builder.Configuration.GetSection("APROconnect:ApiClientSettings"));


builder.Services.AddScoped<IApiClientService, ApiClientService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthorizer, Authorizer>();
builder.Services.AddSingleton<ILocationIdService, LocationIdService>();
builder.Services.AddSingleton<IScheduleStorageService, ScheduleStorageService>();
builder.Services.AddSingleton<IContractPositionStorageService, ContractPositionStorageService>();
builder.Services.AddSingleton<IClientStorageService, ClientStorageService>();
builder.Services.AddSingleton<IContractStorageService, ContractStorageService>();
builder.Services.AddSingleton<IStorageService, StorageService>();
builder.Services.AddSingleton<IConverterToStorageService, ConverterToStorageService>();
builder.Services.AddTransient<IGetterServiceFactory<DataResponse>, DataGetterServiceFactory>(); 
builder.Services.AddTransient<IGetterLocationServiceFactory<LocationDataResponse>, LocationGetterServiceFactory>();
builder.Services.AddTransient<IGetterLocationService<LocationDataResponse>, LocationGetterService>();
builder.Services.AddTransient<ISetterServiceFactory<LocationDataResponse>, LocationSetterServiceFactory>();
builder.Services.AddTransient<ISetterService<LocationDataResponse>, LocationSetterService>();
builder.Services.AddScoped<ILocationMapper, LocationMapper>();
builder.Services.AddScoped<ILocationValidator, LocationValidator>();
builder.Services.AddTransient<IGetterServiceFactory<ScheduleDataResponse>, ScheduleGetterServiceFactory>();
builder.Services.AddTransient<IGetterService<ScheduleDataResponse>, ScheduleGetterService>();
builder.Services.AddTransient<IGetterServiceFactory<ContractPositionDataResponse>, ContractPositionGetterServiceFactory>();
builder.Services.AddTransient<IGetterService<ContractPositionDataResponse>, ContractPositionGetterService>();
builder.Services.AddTransient<IGetterServiceFactory<ContractDataResponseResponse>, ContractGetterServiceFactory>();
builder.Services.AddTransient<IGetterService<ContractDataResponseResponse>, ContractGetterService>();
builder.Services.AddTransient<IGetterServiceFactory<ClientDataResponseResponse>, ClientGetterServiceFactory>();
builder.Services.AddTransient<IGetterService<ClientDataResponseResponse>, ClientGetterService>();

builder.Services.AddScoped<IIntegrationProcessor<ClientDataResponseResponse>, ContragentProcessor>();
builder.Services.AddScoped<IIntegrationProcessor<EmitterDataResponse>, EmitterProcessor>();
builder.Services.AddScoped<IIntegrationProcessor<LocationDataResponse>, LocationProcessor>();
builder.Services.AddScoped<IIntegrationProcessor<ScheduleDataResponse>, ScheduleProcessor>();

builder.Services.AddScoped<IApiClientService, ApiClientService>();
builder.Services.AddScoped<IIntegrationService, IntegrationService>();
builder.Services.AddScoped<ICheckUpFactory<ClientDataResponseResponse>, ClientCheckUpFactory>();
builder.Services.AddScoped<ICheckUpService<ClientDataResponseResponse>, ClientCheckUpService>();


builder.Services.AddSingleton<TokenController>();
builder.Services.AddSingleton<LocationController>();
builder.Services.AddSingleton<ScheduleController>();
builder.Services.AddSingleton<ContractPositionController>();
builder.Services.AddSingleton<ContractController>();
builder.Services.AddSingleton<ClientController>();
builder.Services.AddSingleton<EmitterController>();
builder.Services.AddSingleton<EmitterControllerMT>();
builder.Services.AddSingleton<WasteSiteEntryController>();
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

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

