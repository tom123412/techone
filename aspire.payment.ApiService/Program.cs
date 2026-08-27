using Asp.Versioning;
using aspire.payment.ApiService.Features.Payments.Create;
using aspire.payment.ApiService.Features.PurchaseOrderLineItems.Create;
using aspire.payment.ApiService.Features.Vendors;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

var cosmosConnectionString = builder.Configuration.GetConnectionString("cosmosdb")
    ?? throw new InvalidOperationException("Connection string 'cosmosdb' was not configured.");
builder.Services.AddDbContext<PaymentsCosmosDbContext>(options =>
    options.UseCosmos(cosmosConnectionString, PaymentsCosmosDbContext.DatabaseId));
builder.Services.AddDbContext<VendorsCosmosDbContext>(options =>
    options.UseCosmos(cosmosConnectionString, VendorsCosmosDbContext.DatabaseId));
builder.Services.AddDbContext<PurchaseOrderLineItemsCosmosDbContext>(options =>
    options.UseCosmos(cosmosConnectionString, PurchaseOrderLineItemsCosmosDbContext.DatabaseId));

builder.Services.AddScoped<IPaymentStore, PaymentCosmosStore>();
builder.Services.AddScoped<IVendorStore, VendorCosmosStore>();
builder.Services.AddScoped<IPurchaseOrderLineItemStore, PurchaseOrderLineItemCosmosStore>();

//builder.Services.AddControllers().AddOData(options =>
//    options
//        .Select()
//        .Filter()
//        .OrderBy()
//        .Count()
//        .SetMaxTop(null)
//        .AddRouteComponents("api", VendorEdmModelConfiguration.GetEdmModel()));

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
    .MapVendorEndpoints()
    .MapCreatePurchaseOrderLineItemEndpoint()
    ;

app.MapDefaultEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
