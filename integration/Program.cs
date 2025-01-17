using integration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using System.Net.Http;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache(); // Добавьте MemoryCache
builder.Services.AddHttpClient();
builder.Services.AddTransient<TokenController>();
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
