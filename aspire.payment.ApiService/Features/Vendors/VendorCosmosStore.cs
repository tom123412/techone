using Microsoft.EntityFrameworkCore;

namespace aspire.payment.ApiService.Features.Vendors;

public interface IVendorStore
{
    Task<VendorDocument> SaveAsync(CreateVendorRequest request, CancellationToken cancellationToken);
    Task<IQueryable<VendorDocument>> QueryAsync(CancellationToken cancellationToken);
    Task<VendorDocument?> GetAsync(string id, CancellationToken cancellationToken);
}

internal sealed class VendorCosmosStore(VendorsCosmosDbContext dbContext) : IVendorStore
{
    public async Task<VendorDocument> SaveAsync(CreateVendorRequest request, CancellationToken cancellationToken)
    {
        var (legalName, abn, organisationType, isSmallMediumEnterprise, isIndigenousSupplier) = request.VendorInformation;

        var document = new VendorDocument
        {
            Id = Guid.NewGuid().ToString("N"),
            ApplicationId = request.ApplicationID,
            VendorInformation = new VendorInformation(null, legalName, abn, organisationType, isSmallMediumEnterprise, isIndigenousSupplier),
            VendorAddress = request.VendorAddress,
            PaymentInformation = request.PaymentInformation,
            ContactInformation = request.ContactInformation,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
        dbContext.Vendors.Add(document);
        await dbContext.SaveChangesAsync(cancellationToken);

        return document;
    }

    public async Task<IQueryable<VendorDocument>> QueryAsync(CancellationToken cancellationToken)
    {
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
        return dbContext.Vendors.AsQueryable();
    }

    public async Task<VendorDocument?> GetAsync(string id, CancellationToken cancellationToken)
    {
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
        return await dbContext.Vendors.FindAsync([id], cancellationToken);
    }
}
