using aspire.payment.TechnologyOne.Features.Vendors;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHttpClient("apiservice", client =>
{
    client.BaseAddress = new Uri("http://apiservice");
});
builder.Services.AddHostedService<VendorProcessorWorker>();

var host = builder.Build();
host.Run();
