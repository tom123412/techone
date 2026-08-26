namespace aspire.payment.ApiService.Features.PurchaseOrderLineItems.Create;

public interface IPurchaseOrderLineItemStore
{
    Task<PurchaseOrderLineItemDocument> SaveAsync(CreatePurchaseOrderLineItemRequest request, CancellationToken cancellationToken);
}

internal sealed class PurchaseOrderLineItemCosmosStore(PurchaseOrderLineItemsCosmosDbContext dbContext) : IPurchaseOrderLineItemStore
{
    public async Task<PurchaseOrderLineItemDocument> SaveAsync(CreatePurchaseOrderLineItemRequest request, CancellationToken cancellationToken)
    {
        var document = new PurchaseOrderLineItemDocument
        {
            Id = Guid.NewGuid().ToString("N"),
            PurchasingLocation = request.PurchasingLocation,
            SublocationCode = request.SublocationCode,
            RequisitionNumber = request.RequisitionNumber,
            ProcessingGroupName = request.ProcessingGroupName,
            OrderNumber = request.OrderNumber,
            Reference = request.Reference,
            RequisitionComment = request.RequisitionComment,
            UserFields = request.UserFields,
            OtherInformation = request.OtherInformation,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
        dbContext.PurchaseOrderLineItems.Add(document);
        await dbContext.SaveChangesAsync(cancellationToken);

        return document;
    }
}
