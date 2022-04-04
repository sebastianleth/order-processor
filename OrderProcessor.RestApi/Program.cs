using Microsoft.OpenApi.Models;
using OrderProcessor;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOrderProcessorServices();
builder.Services.AddHostedService<OrderProcessor.Processing.ProcessingHostedService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Order Processor API",
        Version = "v1"
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();
