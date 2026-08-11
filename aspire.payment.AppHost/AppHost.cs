var builder = DistributedApplication.CreateBuilder(args);

builder.AddAzureContainerAppEnvironment("aca-env");

var cosmosDb = builder.AddAzureCosmosDB("cosmosdb");

var apiService = builder.AddProject<Projects.aspire_payment_ApiService>("apiservice")
    .WithHttpHealthCheck("/health")
    .WithReference(cosmosDb);

builder.AddProject<Projects.aspire_payment_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
