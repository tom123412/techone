using Asp.Versioning;
using aspire.payment.ApiService.Features.Payments.Create;
using aspire.payment.ApiService.Features.PurchaseOrderItems.Create;
using aspire.payment.ApiService.Features.SupplierForms.Create;
using Microsoft.Azure.Cosmos;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();
builder.AddAzureCosmosClient("cosmosdb", (settings) => { }, (options) =>
{
    options.SerializerOptions = new CosmosSerializationOptions
    {
        PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
    };
});

// Add services to the container.
builder.Services.AddProblemDetails();
builder.Services.AddSingleton<IPaymentStore, PaymentCosmosStore>();
builder.Services.AddSingleton<ISupplierFormStore, SupplierFormCosmosStore>();
builder.Services.AddSingleton<IPurchaseOrderItemStore, PurchaseOrderItemCosmosStore>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "API v1");
    });
}

string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

app.MapGet("/", () => "API service is running. Navigate to /api/v1/weatherforecast to see sample data.");

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
;

app
    .MapCreatePaymentEndpoint()
    .MapCreateSupplierFormEndpoint()
    .MapCreatePurchaseOrderItemEndpoint()
    ;

app.MapDefaultEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
