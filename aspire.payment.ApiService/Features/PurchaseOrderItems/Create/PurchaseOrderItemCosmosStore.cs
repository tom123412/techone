using aspire.payment.ApiService.Persistence;

namespace aspire.payment.ApiService.Features.PurchaseOrderItems.Create;

public interface IPurchaseOrderItemStore
{
    Task<PurchaseOrderItemDocument> SaveAsync(CreatePurchaseOrderItemRequest request, CancellationToken cancellationToken);
}

internal sealed class PurchaseOrderItemCosmosStore(PaymentsCosmosDbContext dbContext) : IPurchaseOrderItemStore
{
    public async Task<PurchaseOrderItemDocument> SaveAsync(CreatePurchaseOrderItemRequest request, CancellationToken cancellationToken)
    {
        var document = new PurchaseOrderItemDocument
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
        dbContext.PurchaseOrderItems.Add(document);
        await dbContext.SaveChangesAsync(cancellationToken);

        return document;
    }
}
