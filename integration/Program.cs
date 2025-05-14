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

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();

builder.Services.AddSingleton<ILocationIdService, LocationIdService>();
builder.Services.AddSingleton<IScheduleStorageService, ScheduleStorageService>();
builder.Services.AddSingleton<IContractPositionStorageService, ContractPositionStorageService>();
builder.Services.AddSingleton<IClientStorageService, ClientStorageService>();
builder.Services.AddSingleton<IContractStorageService, ContractStorageService>();
builder.Services.AddSingleton<IStorageService, StorageService>();
builder.Services.AddSingleton<IConverterToStorageService, ConverterToStorageService>();
builder.Services.AddTransient<IGetterServiceFactory<Data>, DataGetterServiceFactory>(); 
builder.Services.AddTransient<IGetterLocationServiceFactory<LocationData>, LocationGetterServiceFactory>();
builder.Services.AddTransient<IGetterLocationService<LocationData>, LocationGetterService>();
builder.Services.AddTransient<ISetterServiceFactory<LocationData>, LocationSetterServiceFactory>();
builder.Services.AddTransient<ISetterService<LocationData>, LocationSetterService>();
builder.Services.AddTransient<IGetterServiceFactory<ScheduleData>, ScheduleGetterServiceFactory>();
builder.Services.AddTransient<IGetterService<ScheduleData>, ScheduleGetterService>();
builder.Services.AddTransient<IGetterServiceFactory<ContractPositionData>, ContractPositionGetterServiceFactory>();
builder.Services.AddTransient<IGetterService<ContractPositionData>, ContractPositionGetterService>();
builder.Services.AddTransient<IGetterServiceFactory<ContractData>, ContractGetterServiceFactory>();
builder.Services.AddTransient<IGetterService<ContractData>, ContractGetterService>();
builder.Services.AddTransient<IGetterServiceFactory<ClientData>, ClientGetterServiceFactory>();
builder.Services.AddTransient<IGetterService<ClientData>, ClientGetterService>();

builder.Services.AddScoped<IIntegrationService, IntegrationService>();
builder.Services.AddScoped<ICheckUpFactory<ClientData>, ClientCheckUpFactory>();
builder.Services.AddScoped<ICheckUpService<ClientData>, ClientCheckUpService>();




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

