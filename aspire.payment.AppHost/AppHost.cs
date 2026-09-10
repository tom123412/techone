using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddAzureContainerAppEnvironment("aca-env");

IResourceBuilder<IResourceWithConnectionString> cosmosAccount;
//var cosmosName = builder.AddParameter("ExistingCosmosAccountName");
//if (!string.IsNullOrEmpty(await cosmosName.Resource.GetValueAsync(CancellationToken.None)))
//{
//    var resourceGroup = builder.AddParameter("ExistingCosmosResourceGroup");

//    cosmosAccount 
////.RunAsEmulator()
//        .ClearDefaultRoleAssignments()
////.PublishAsExisting(cosmosName, resourceGroup)
//        .RunAsExisting(cosmosName, resourceGroup)
//        ;
//}

if (builder.Environment.IsDevelopment())
{
    cosmosAccount = builder.AddConnectionString("cosmos-account");
}
else
{
    cosmosAccount = builder.AddAzureCosmosDB("cosmos-account");
    var cosmosDb = (cosmosAccount as IResourceBuilder<AzureCosmosDBResource>)!.AddCosmosDatabase("cosmos-db");
    var payments = cosmosDb.AddContainer("payments", "/id");
    var vendors = cosmosDb.AddContainer("vendors", "/id");
    var purchaseOrderLineItems = cosmosDb.AddContainer("purchase-order-line-items", "/id");

}

var logAnalytics = builder.AddAzureLogAnalyticsWorkspace("logs");
var azureMonitor = builder.AddAzureApplicationInsights("azure-monitor", logAnalytics);

var apiService = builder.AddProject<Projects.aspire_payment_ApiService>("apiservice")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cosmosAccount)
    //.WithReference(vendors)
    //.WithReference(purchaseOrderLineItems)
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
