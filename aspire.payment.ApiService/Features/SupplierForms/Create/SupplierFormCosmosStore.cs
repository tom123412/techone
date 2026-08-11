using Microsoft.Azure.Cosmos;

namespace aspire.payment.ApiService.Features.SupplierForms.Create;

public interface ISupplierFormStore
{
    Task<SupplierFormDocument> SaveAsync(CreateSupplierFormRequest request, CancellationToken cancellationToken);
}

internal sealed class SupplierFormCosmosStore(CosmosClient cosmosClient) : ISupplierFormStore
{
    private const string DatabaseId = "payments";
    private const string ContainerId = "supplierforms";

    public async Task<SupplierFormDocument> SaveAsync(CreateSupplierFormRequest request, CancellationToken cancellationToken)
    {
        var document = new SupplierFormDocument(
            Guid.NewGuid().ToString("N"),
            request.ApplicationID,
            request.SupplierPartyInformation,
            request.SupplierAddress,
            request.PaymentInformation,
            DateTimeOffset.UtcNow);

        var databaseResponse = await cosmosClient.CreateDatabaseIfNotExistsAsync(DatabaseId, cancellationToken: cancellationToken);
        var containerResponse = await databaseResponse.Database.CreateContainerIfNotExistsAsync(ContainerId, "/applicationId", cancellationToken: cancellationToken);

        await containerResponse.Container.UpsertItemAsync(document, new PartitionKey(document.ApplicationId), cancellationToken: cancellationToken);

        return document;
    }
}
