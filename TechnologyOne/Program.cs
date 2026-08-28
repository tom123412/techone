using aspire.payment.TechnologyOne.Features.Vendors;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<VendorProcessorWorker>();

var host = builder.Build();
host.Run();
