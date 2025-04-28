using integration;
<<<<<<< Updated upstream
using integration.Controllers;
using integration.Controllers.Apro;
using integration.Controllers.MT;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
=======
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using integration.Services.Interfaces;
using integration.Services.Location;
using integration.Services.Schedule;
using integration.Services.Client;
using integration.Services.ContractPosition;
using integration.Services.Storage;
using integration.Services.Client.Storage;
using integration.Services.ContractPosition.Storage;
using Microsoft.Extensions.Configuration;
>>>>>>> Stashed changes
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

<<<<<<< Updated upstream
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();
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
=======
// Add logging (Crucially Important)
builder.Logging.AddConsole(); // Or AddDebug(), AddFile(), etc.  Choose your desired provider

builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();

// Register TokenService as Scoped
builder.Services.AddScoped<ITokenService, TokenService>();

// Register all *Services* as Scoped
builder.Services.AddScoped<ILocationIdService, LocationIdService>();
builder.Services.AddScoped<IScheduleStorageService, ScheduleStorageService>();
builder.Services.AddScoped<IContractPositionStorageService, ContractPositionStorageService>();
builder.Services.AddScoped<IClientStorageService, ClientStorageService>();
builder.Services.AddScoped<IContractStorageService, ContractStorageService>();
builder.Services.AddScoped<IStorageService, StorageService>();
builder.Services.AddScoped<IConverterToStorageService, ConverterToStorageService>(); //Keep scoped if using DbContext
// or change to transient

// Removed Controller Registrations

builder.Services.AddHostedService<MainSyncService>(); // Keep HostedService as Singleton
>>>>>>> Stashed changes
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer();
builder.Services.AddAuthorization();


<<<<<<< Updated upstream
var app = builder.Build();
/*
=======
// Configure the HTTP request pipeline.
>>>>>>> Stashed changes
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
<<<<<<< Updated upstream
*/
=======

>>>>>>> Stashed changes
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
