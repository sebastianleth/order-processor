using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using OrderProcessor;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddOrderProcessorServices();
builder.Services.AddHostedService<OrderProcessor.Processing.ProcessingHostedService>();

var serializerSettings = new JsonSerializerSettings();

builder.Services
    .AddControllers()
    .AddNewtonsoftJson(options =>
    {
        serializerSettings = options.SerializerSettings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenNewtonsoftSupport();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Order Processor API",
        Version = "v1"
    });

    options.ConfigureForNodaTime(serializerSettings);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
