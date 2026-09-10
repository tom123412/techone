using aspire.payment.TechnologyOne.Features.Vendors;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.Configure<VendorOptions>(
    builder.Configuration.GetSection(VendorOptions.SectionName));
builder.Services.Configure<FieldMappingsOptions>(
    builder.Configuration.GetSection(FieldMappingsOptions.SectionName));
builder.Services.AddHttpClient("apiservice", client =>
{
    client.BaseAddress = new Uri("http://apiservice");
});
builder.Services.AddHostedService<VendorProcessIncomingCsvWorker>();
builder.Services.AddSingleton<VendorReadyToExportWorker>();
builder.Services.AddHostedService(provider => provider.GetRequiredService<VendorReadyToExportWorker>());
builder.Services.AddHostedService<VendorGenerateIncomingCsvWorker>();

var app = builder.Build();

app.MapPost("/api/vendors/events", async (VendorCreatedEventPayload vendorCreatedEvent, VendorReadyToExportWorker worker, CancellationToken cancellationToken) =>
{
    await worker.ProcessVendorCreatedEventAsync(vendorCreatedEvent, cancellationToken);
    return Results.Accepted();
});

app.MapDefaultEndpoints();

app.Run();
