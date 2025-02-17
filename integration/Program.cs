using integration;
using integration.Context;
using integration.Controllers;
using integration.Controllers.Apro;
using integration.Controllers.MT;
using integration.HelpClasses;
using integration.Services;
using integration.Services.Factory;
using integration.Services.Factory.Interfaces;
using integration.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();

// Register the factories
builder.Services.AddTransient<IGetterServiceFactory<Data>, DataGetterServiceFactory>(); 
builder.Services.AddTransient<IGetterServiceFactory<LocationData>, LocationGetterServiceFactory>();
builder.Services.AddTransient<IGetterService<LocationData>, LocationGetterService>();


builder.Services.AddSingleton<TokenController>();
builder.Services.AddSingleton<ClientController>();
builder.Services.AddSingleton<EmitterController>();
builder.Services.AddSingleton<EmitterControllerMT>();
builder.Services.AddSingleton<WasteSiteEntryController>();
builder.Services.AddSingleton<ScheduleController>();
builder.Services.AddSingleton<EntryController>();
builder.Services.AddSingleton<LocationController>();
builder.Services.AddSingleton<ScheduleController>();
builder.Services.AddHostedService<DataSyncService>();
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
