using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration; // Добавляем using для IConfiguration
using Microsoft.Extensions.Hosting;
using Serilog; //Добавляем using для Serilog
using Serilog.Events;
using System.Net;
using integration;
using System;
using integration.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.File(path: "C:\\Users/zubcova_ma/Desktop/logs/logs.txt", outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}", rollingInterval: RollingInterval.Infinite)
    .CreateLogger();

builder.Services.AddMemoryCache(); // Добавьте MemoryCache

builder.Services.AddHttpClient();
builder.Services.AddSingleton<TokenController>();
//builder.Services.AddTransient<DataController>();
builder.Services.AddControllers();
//builder.Services.AddHostedService<TokenRefreshService>();
// Добавляем Interceptor
builder.Services.AddTransient<AuthHeaderHandler>();

builder.Services.AddHttpClient("AuthenticatedClient")
    .AddHttpMessageHandler<AuthHeaderHandler>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setup =>
{
    // Include 'SecurityScheme' to use JWT Authentication
    //var jwtSecurityScheme = new OpenApiSecurityScheme
    //{
    //    BearerFormat = "JWT",
    //    Name = "JWT Authentication",
    //    In = ParameterLocation.Header,
    //    Type = SecuritySchemeType.Http,
    //    Scheme = JwtBearerDefaults.AuthenticationScheme,
    //    Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

    //    Reference = new OpenApiReference
    //    {
    //        Id = JwtBearerDefaults.AuthenticationScheme,
    //        Type = ReferenceType.SecurityScheme
    //    }
    //};

 /*   setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    setup.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });*/

});

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer();
builder.Services.AddAuthorization();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
