using Microsoft.Azure.Cosmos;

namespace aspire.payment.ApiService.Features.Payments.Create;

public interface IPaymentStore
{
    Task<PaymentDocument> SaveAsync(CreatePaymentRequest request, CancellationToken cancellationToken);
}

internal sealed class PaymentCosmosStore(CosmosClient cosmosClient) : IPaymentStore
{
    private const string DatabaseId = "payments";
    private const string ContainerId = "payments";

    public async Task<PaymentDocument> SaveAsync(CreatePaymentRequest request, CancellationToken cancellationToken)
    {
        var document = new PaymentDocument(
            Guid.NewGuid().ToString("N"),
            request.InvoiceDate,
            request.LedgerCode,
            request.AccountNumber,
            request.GSTExclusiveAmount,
            request.GSTAmount,
            request.GSTInclusiveAmount,
            request.InvoiceNarration1,
            request.InvoiceNarration2,
            request.InvoiceNarration3,
            request.BID,
            request.PurchaseLocation,
            request.PurchaseOrderNumber,
            request.GeneralLedgerCode,
            request.RegisteredForGST,
            request.ApplicationID,
            request.RequestedBy,
            DateTimeOffset.UtcNow);

        var databaseResponse = await cosmosClient.CreateDatabaseIfNotExistsAsync(DatabaseId, cancellationToken: cancellationToken);
        var containerResponse = await databaseResponse.Database.CreateContainerIfNotExistsAsync(ContainerId, "/id", cancellationToken: cancellationToken);

        await containerResponse.Container.UpsertItemAsync(document, new PartitionKey(document.Id), cancellationToken: cancellationToken);

        return document;
    }
}
