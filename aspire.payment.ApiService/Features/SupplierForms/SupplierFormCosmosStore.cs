namespace aspire.payment.ApiService.Features.SupplierForms;

public interface ISupplierFormStore
{
    Task<SupplierFormDocument> SaveAsync(CreateSupplierFormRequest request, CancellationToken cancellationToken);
    Task<SupplierFormDocument?> GetAsync(string id, CancellationToken cancellationToken);
}

internal sealed class SupplierFormCosmosStore(SupplierFormsCosmosDbContext dbContext) : ISupplierFormStore
{
    public async Task<SupplierFormDocument> SaveAsync(CreateSupplierFormRequest request, CancellationToken cancellationToken)
    {
        var document = new SupplierFormDocument
        {
            Id = Guid.NewGuid().ToString("N"),
            ApplicationId = request.ApplicationID,
            SupplierPartyInformation = request.SupplierPartyInformation,
            SupplierAddress = request.SupplierAddress,
            PaymentInformation = request.PaymentInformation,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
        dbContext.SupplierForms.Add(document);
        await dbContext.SaveChangesAsync(cancellationToken);

        return document;
    }

    public async Task<SupplierFormDocument?> GetAsync(string id, CancellationToken cancellationToken)
    {
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
        return await dbContext.SupplierForms.FindAsync([id], cancellationToken);
    }
}
