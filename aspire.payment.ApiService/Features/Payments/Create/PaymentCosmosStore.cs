namespace aspire.payment.ApiService.Features.Payments.Create;

public interface IPaymentStore
{
    Task<PaymentDocument> SaveAsync(CreatePaymentRequest request, CancellationToken cancellationToken);
}

internal sealed class PaymentCosmosStore(PaymentsCosmosDbContext dbContext) : IPaymentStore
{
    async Task<PaymentDocument> IPaymentStore.SaveAsync(CreatePaymentRequest request, CancellationToken cancellationToken)
    {
        var document = new PaymentDocument
        {
            Id = Guid.NewGuid().ToString("N"),
            InvoiceDate = request.InvoiceDate,
            LedgerCode = request.LedgerCode,
            AccountNumber = request.AccountNumber,
            GSTExclusiveAmount = request.GSTExclusiveAmount,
            GSTAmount = request.GSTAmount,
            GSTInclusiveAmount = request.GSTInclusiveAmount,
            InvoiceNarration1 = request.InvoiceNarration1,
            InvoiceNarration2 = request.InvoiceNarration2,
            InvoiceNarration3 = request.InvoiceNarration3,
            BID = request.BID,
            PurchaseLocation = request.PurchaseLocation,
            PurchaseOrderNumber = request.PurchaseOrderNumber,
            GeneralLedgerCode = request.GeneralLedgerCode,
            RegisteredForGST = request.RegisteredForGST,
            ApplicationID = request.ApplicationID,
            RequestedBy = request.RequestedBy,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
        dbContext.Payments.Add(document);
        await dbContext.SaveChangesAsync(cancellationToken);

        return document;
    }
}
