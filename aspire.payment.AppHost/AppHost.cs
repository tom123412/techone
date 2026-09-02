var builder = DistributedApplication.CreateBuilder(args);

builder.AddAzureContainerAppEnvironment("aca-env");

var cosmosName = builder.AddParameter("ExistingCosmosAccountName");
var resourceGroup = builder.AddParameter("ExistingCosmosResourceGroup");

var cosmosDb = builder
    .AddAzureCosmosDB("cosmos-db")
    //.RunAsEmulator()
    .ClearDefaultRoleAssignments()
    //.PublishAsExisting(cosmosName, resourceGroup)
    .RunAsExisting(cosmosName, resourceGroup)
    ;

//var cosmosAccount = builder.AddAzureCosmosDB("cosmos-account");
//var cosmosDb = cosmosAccount.AddCosmosDatabase("cosmos-db");

var apiService = builder.AddProject<Projects.aspire_payment_ApiService>("apiservice")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cosmosDb)
    ;

builder.AddProject<Projects.aspire_payment_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService)
    ;

builder.AddProject<Projects.aspire_payment_TechnologyOne>("technologyone")
    .WithReference(apiService)
    .WaitFor(apiService)
    ;

builder.Build().Run();
