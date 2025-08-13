using integration;
using integration.Context;
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
using integration.Context.Request;
using integration.Context.Request.MT;
using integration.Context.Response;
using integration.Factory.GET.MT;
using integration.Infrastructure;
using integration.Services.Agre;
using integration.Services.Agre.Storage;
using integration.Services.Container;
using integration.Services.Emitter;
using integration.Services.Emitter.Storage;
using integration.Services.Employers;
using integration.Services.Employers.Storage;
using integration.Services.Entry;
using integration.Services.Entry.MT;
using integration.Services.Entry.MT.Storage;
using integration.Services.Entry.Storage;
using integration.Services.Location.fromMT;
using integration.Services.Location.fromMT.Storage;
using integration.Services.Storage.Interfaces;
using integration.Structs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Добавление сервисов
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
builder.Services.AddLogging(logging => 
{
    logging.AddConsole();
    logging.AddDebug();
    logging.SetMinimumLevel(LogLevel.Debug);
});

// Конфигурация
builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection("AuthSettings"));
builder.Services.Configure<ApiClientSettings>(builder.Configuration.GetSection("APROconnect:ApiClientSettings"));

// Регистрация DbContext - ИСПРАВЛЕННАЯ
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")),
    ServiceLifetime.Scoped);

// Регистрация сервисов - ВСЕ СЕРВИСЫ РАБОТАЮЩИЕ С БД ДОЛЖНЫ БЫТЬ SCOPED

builder.Services.AddScoped<IApiClientService, ApiClientService>();
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddScoped<ITokenManagerService, TokenManagerService>();
builder.Services.AddSingleton<IAuthorizer, Authorizer>();
builder.Services.AddScoped<ILocationIdService, LocationIdService>();

// Регистрация sync-сервисов
// Регистрация sync-сервисов
builder.Services.AddScoped<IAgreManagerService, AgreManagerService>();
builder.Services.AddScoped<IClientManagerService, ClientManagerService>();
builder.Services.AddScoped<IContractManagerService, ContractManagerService>();
builder.Services.AddScoped<IContractPositionManagerService, ContractPositionManagerService>();
builder.Services.AddScoped<IEmitterManagerService, EmitterManagerService>();
builder.Services.AddScoped<IEmployerManagerService, EmployerManagerService>();
builder.Services.AddScoped<IEntryManagerService, EntryManagerService>();
builder.Services.AddScoped<ILocationManagerService, LocationManagerService>();
builder.Services.AddScoped<IScheduleManagerService, ScheduleManagerService>();
builder.Services.AddScoped<IIntegrationService, IntegrationService>();

// Storage services - ВСЕ Scoped
builder.Services.AddScoped<IAgreStorageService, AgreStorageService>();
builder.Services.AddScoped<IEntryStorageService<EntryDataResponse>, EntryStorageService>();
builder.Services.AddScoped<IEntryStorageService<EntryMTDataResponse>, EntryMTStorageService>();
builder.Services.AddScoped<ILocationMTStorageService, LocationMTStorageService>();
builder.Services.AddScoped<ILocationMTStatusStorageService, LocationMtStatusStorageStorageService>();
builder.Services.AddScoped<IScheduleStorageService, ScheduleStorageService>();
builder.Services.AddScoped<IContractPositionStorageService, ContractPositionStorageService>();
builder.Services.AddScoped<IClientStorageService, ClientStorageService>();
builder.Services.AddScoped<IContractStorageService, ContractStorageService>();
builder.Services.AddScoped<IStorageService<IntegrationStruct>, StorageService>();
builder.Services.AddScoped<IConverterToStorageService, ConverterToStorageService>();
builder.Services.AddScoped<IEmitterStorageService, EmitterStorageService>();
builder.Services.AddScoped<IEmployersStorageService, EmployersStorageService>();

// Фабрики и сервисы получения данных
builder.Services.AddTransient<IGetterServiceFactory<EmployerDataResponse>, EmployersGetterServiceFactory>();
builder.Services.AddTransient<IGetterService<EmployerDataResponse>, EmployerGetterService>();
builder.Services.AddTransient<IGetterServiceFactory<AgreMTDataResponse>, AgreMTGetterServiceFactory>();
builder.Services.AddTransient<IGetterService<AgreMTDataResponse>, AgreMTGetterService>();
builder.Services.AddTransient<ISetterServiceFactory<AgreRequest>, AgreSetterServiceFactory>();
builder.Services.AddTransient<ISetterService<AgreRequest>, AgreSetterService>();
builder.Services.AddTransient<IGetterServiceFactory<DataResponse>, DataGetterServiceFactory>(); 
builder.Services.AddTransient<IGetterLocationServiceFactory<LocationDataResponse>, LocationGetterServiceFactory>();
builder.Services.AddTransient<IGetterLocationService<LocationDataResponse>, LocationGetterService>();
builder.Services.AddTransient<IGetterServiceFactory<LocationMTPhotoDataResponse>, LocationMTPhotoGetterServiceFactory>();
builder.Services.AddTransient<IGetterService<LocationMTPhotoDataResponse>, LocationMTPhotoGetterService>();
builder.Services.AddTransient<IGetterServiceFactory<LocationMTDataResponse>, LocationMtGetterServiceFactory>();
builder.Services.AddTransient<IGetterService<LocationMTDataResponse>, LocationMTGetterService>();
builder.Services.AddTransient<ISetterServiceFactory<LocationMTDataResponse>, LocationFromMTSetterServiceFactory>();
builder.Services.AddTransient<ISetterService<LocationMTDataResponse>, LocationFromMTSetterService>();
builder.Services.AddTransient<IGetterServiceFactory<Container>, ContainerGetterServiceFactory>();
builder.Services.AddTransient<IGetterService<Container>, ContainerGetterService>();
builder.Services.AddTransient<ISetterServiceFactory<LocationDataResponse>, LocationSetterServiceFactory>();
builder.Services.AddTransient<ISetterService<LocationDataResponse>, LocationSetterService>();
builder.Services.AddTransient<ISetterServiceFactory<EntryDataResponse>, EntrySetterServiceFactory>();
builder.Services.AddTransient<IGetterServiceFactory<EntryMTDataResponse>, EntryMTGetterServiceFactory>();
builder.Services.AddTransient<ISetterService<EntryDataResponse>, EntrySetterService>();
builder.Services.AddTransient<ISetterServiceFactory<EntryMTRequest>, EntryFromMTServiceFactory>();
builder.Services.AddTransient<ISetterService<EntryMTRequest>, EntryFromMTSetterService>();
builder.Services.AddTransient<ISetterServiceFactory<LocationMTPhotoDataResponse>, LocationFromMTPhotoSetterServiceFactory>();
builder.Services.AddTransient<ISetterService<LocationMTPhotoDataResponse>, LocationFromMTPhotoSetterService>();

// Сервисы валидации и проверок
builder.Services.AddScoped<ILocationValidator, LocationValidator>();
builder.Services.AddScoped<IIntegrationValidationService, IntegrationValidationService>();
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
builder.Services.AddTransient<IGetterServiceFactory<LocationMTPhotoDataResponse>, LocationMTPhotoGetterServiceFactory>();
builder.Services.AddTransient<IGetterService<LocationMTPhotoDataResponse>, LocationMTPhotoGetterService>();
builder.Services.AddScoped<IClientCheckUpService, ClientCheckUpService>();
builder.Services.AddScoped<IEmitterCheckUpService, EmitterCheckUpService>();
builder.Services.AddScoped<ILocationCheckUpService, LocationCheckUpService>();
builder.Services.AddScoped<IScheduleCheckUpService, ScheduleCheckUpService>();

// Интеграционные сервисы
builder.Services.AddScoped<IntegrationStructValidator>();
builder.Services.AddScoped<IIntegrationProcessor<ClientDataResponse>, ContragentProcessor>();
builder.Services.AddScoped<IIntegrationProcessor<EmitterDataResponse>, EmitterProcessor>();
builder.Services.AddScoped<IIntegrationProcessor<LocationDataResponse>, LocationProcessor>();
builder.Services.AddScoped<IIntegrationProcessor<ScheduleDataResponse>, ScheduleProcessor>();
builder.Services.AddScoped<IAproClientService, AproClientService>();
builder.Services.AddScoped<ICheckUpFactory<ClientDataResponse>, ClientCheckUpFactory>();
builder.Services.AddScoped<ICheckUpService<ClientDataResponse>, ClientCheckUpService>();
builder.Services.AddScoped<ICheckUpFactory<EmitterDataResponse>, EmitterCheckUpFactory>();
builder.Services.AddScoped<ICheckUpService<EmitterDataResponse>, EmitterCheckUpService>();
builder.Services.AddScoped<ICheckUpFactory<ScheduleDataResponse>, ScheduleCheckUpFactory>();
builder.Services.AddScoped<ICheckUpService<ScheduleDataResponse>, ScheduleCheckUpService>();
builder.Services.AddScoped<ICheckUpFactory<LocationDataResponse>, LocationCheckUpFactory>();
builder.Services.AddScoped<ICheckUpService<LocationDataResponse>, LocationCheckUpService>();

// Фоновый сервис с правильной обработкой Scoped-зависимостей - ИСПРАВЛЕННЫЙ
builder.Services.AddHostedService<MainSyncService>();

// Автоматическая регистрация контроллеров
builder.Services.AddControllers();

// Swagger и API Explorer
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Аутентификация и авторизация
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer();
builder.Services.AddAuthorization();

// AutoMapper
var mapperConfig = new MapperConfiguration(cfg => 
{
    cfg.AddProfile<IntegrationMappingProfile>();
});
builder.Services.AddSingleton<IMapper>(sp => mapperConfig.CreateMapper());

var app = builder.Build();

// Конвейер middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Применение миграций
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while applying migrations");
    }
}

app.Run();