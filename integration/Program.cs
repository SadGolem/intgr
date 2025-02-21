using integration;
using integration.Context;
using integration.Controllers;
using integration.Controllers.Apro;
using integration.Controllers.MT;
using integration.Factory.GET;
using integration.Factory.GET.Interfaces;
using integration.Factory.SET;
using integration.Factory.SET.Interfaces;
using integration.Services.ContractPosition;
using integration.Services.Interfaces;
using integration.Services.Location;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();

builder.Services.AddSingleton<ILocationIdService, LocationIdService>();
builder.Services.AddTransient<IGetterServiceFactory<Data>, DataGetterServiceFactory>(); 
builder.Services.AddTransient<IGetterLocationServiceFactory<LocationData>, LocationGetterServiceFactory>();
builder.Services.AddTransient<IGetterLocationService<LocationData>, LocationGetterService>();
builder.Services.AddTransient<ISetterServiceFactory<LocationData>, LocationSetterServiceFactory>();
builder.Services.AddTransient<ISetterService<LocationData>, LocationSetterService>();
builder.Services.AddTransient<IGetterServiceFactory<ContractData>, ContractPositionGetterServiceFactory>();
builder.Services.AddTransient<IGetterService<ContractData>, ContractPositionGetterService>();

builder.Services.AddSingleton<TokenController>();
builder.Services.AddSingleton<LocationController>();
builder.Services.AddSingleton<ContractPositionController>();
builder.Services.AddSingleton<ClientController>();
builder.Services.AddSingleton<EmitterController>();
builder.Services.AddSingleton<EmitterControllerMT>();
builder.Services.AddSingleton<WasteSiteEntryController>();
builder.Services.AddSingleton<ScheduleController>();
builder.Services.AddSingleton<EntryController>();
builder.Services.AddSingleton<ScheduleController>();
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
