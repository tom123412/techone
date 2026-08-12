using Microsoft.Azure.Cosmos;

namespace aspire.payment.ApiService.Features.PurchaseOrderItems.Create;

public interface IPurchaseOrderItemStore
{
    Task<PurchaseOrderItemDocument> SaveAsync(CreatePurchaseOrderItemRequest request, CancellationToken cancellationToken);
}

internal sealed class PurchaseOrderItemCosmosStore(CosmosClient cosmosClient) : IPurchaseOrderItemStore
{
    private const string DatabaseId = "payments";
    private const string ContainerId = "purchaseorderitems";

    public async Task<PurchaseOrderItemDocument> SaveAsync(CreatePurchaseOrderItemRequest request, CancellationToken cancellationToken)
    {
        var document = new PurchaseOrderItemDocument(
            Guid.NewGuid().ToString("N"),
            request.PurchasingLocation,
            request.SublocationCode,
            request.RequisitionNumber,
            request.ProcessingGroupName,
            request.OrderNumber,
            request.Reference,
            request.RequisitionComment,
            request.UserFields,
            request.OtherInformation,
            DateTimeOffset.UtcNow);

        var databaseResponse = await cosmosClient.CreateDatabaseIfNotExistsAsync(DatabaseId, cancellationToken: cancellationToken);
        var containerResponse = await databaseResponse.Database.CreateContainerIfNotExistsAsync(ContainerId, "/id", cancellationToken: cancellationToken);

        await containerResponse.Container.UpsertItemAsync(document, new PartitionKey(document.Id), cancellationToken: cancellationToken);

        return document;
    }
}
