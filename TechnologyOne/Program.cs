using aspire.payment.TechnologyOne.Features.Vendors;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.Configure<VendorOptions>(
    builder.Configuration.GetSection(VendorOptions.SectionName));
builder.Services.AddHttpClient("apiservice", client =>
{
    client.BaseAddress = new Uri("http://apiservice");
});
builder.Services.AddHostedService<VendorProcessCsvWorker>();
builder.Services.AddHostedService<VendorReadyToExportWorker>();
builder.Services.AddHostedService<VendorIncomingCsvWorker>();

var host = builder.Build();
host.Run();
