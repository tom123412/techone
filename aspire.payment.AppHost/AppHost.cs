using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddAzureContainerAppEnvironment("aca-env");

var cosmosAccount = builder.AddAzureCosmosDB("cosmos-account");
var cosmosDb = cosmosAccount.AddCosmosDatabase("cosmos-db");
    
var payments = cosmosDb.AddContainer("payments", "/id");
var vendors = cosmosDb.AddContainer("vendors", "/id");
var purchaseOrderLineItems = cosmosDb.AddContainer("purchase-order-line-items", "/id");

if (builder.Environment.IsDevelopment())
{
    var cosmosName = builder.AddParameter("ExistingCosmosAccountName");
    var resourceGroup = builder.AddParameter("ExistingCosmosResourceGroup");

    cosmosAccount 
//.RunAsEmulator()
        .ClearDefaultRoleAssignments()
//.PublishAsExisting(cosmosName, resourceGroup)
        .RunAsExisting(cosmosName, resourceGroup)
        ;
}

var logAnalytics = builder.AddAzureLogAnalyticsWorkspace("logs");
var azureMonitor = builder.AddAzureApplicationInsights("azure-monitor", logAnalytics);

var apiService = builder.AddProject<Projects.aspire_payment_ApiService>("apiservice")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(payments)
    .WithReference(vendors)
    .WithReference(purchaseOrderLineItems)
    .WithReference(azureMonitor)
    ;

builder.AddProject<Projects.aspire_payment_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService)
    ;

builder.AddProject<Projects.aspire_payment_TechnologyOne>("technologyone")
    .WithReference(apiService)
    .WithReference(azureMonitor)
    .WaitFor(apiService)
    ;

builder.Build().Run();
