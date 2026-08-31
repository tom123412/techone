using aspire.payment.TechnologyOne.Features.Vendors;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.Configure<VendorExportOptions>(
    builder.Configuration.GetSection(VendorExportOptions.SectionName));
builder.Services.AddHttpClient("apiservice", client =>
{
    client.BaseAddress = new Uri("http://apiservice");
});
builder.Services.AddHostedService<VendorProcessCsvWorker>();
builder.Services.AddHostedService<VendorReadyToExportWorker>();

var host = builder.Build();
host.Run();
