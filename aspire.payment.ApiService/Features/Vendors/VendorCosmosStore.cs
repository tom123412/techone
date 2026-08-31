using Microsoft.EntityFrameworkCore;

namespace aspire.payment.ApiService.Features.Vendors;

public interface IVendorStore
{
    Task<VendorDocument> CreateAsync(CreateVendorRequest request, CancellationToken cancellationToken);
    Task<IQueryable<VendorDocument>> QueryAsync(CancellationToken cancellationToken);
    Task<VendorDocument?> GetAsync(string id, CancellationToken cancellationToken);
    Task<VendorDocument?> PatchAsync(string id, PatchVendorRequest request, CancellationToken cancellationToken);
}

internal sealed class VendorCosmosStore(VendorsCosmosDbContext dbContext) : IVendorStore
{
    async Task<VendorDocument> IVendorStore.CreateAsync(CreateVendorRequest request, CancellationToken cancellationToken)
    {
        var (legalName, abn, organisationType, isSmallMediumEnterprise, isIndigenousSupplier) = request.VendorInformation;

        var document = new VendorDocument
        {
            Id = Guid.NewGuid().ToString("N"),
            ApplicationId = request.ApplicationID,
            VendorInformation = new VendorInformation(null, legalName, abn, organisationType, isSmallMediumEnterprise, isIndigenousSupplier, Status.ReadyForExport),
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

    async Task<IQueryable<VendorDocument>> IVendorStore.QueryAsync(CancellationToken cancellationToken)
    {
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
        return dbContext.Vendors.AsQueryable();
    }

    async Task<VendorDocument?> IVendorStore.GetAsync(string id, CancellationToken cancellationToken)
    {
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
        return await dbContext.Vendors.FindAsync([id], cancellationToken);
    }

    async Task<VendorDocument?> IVendorStore.PatchAsync(string id, PatchVendorRequest request, CancellationToken cancellationToken)
    {
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        var document = await dbContext.Vendors.FindAsync([id], cancellationToken);
        if (document is null)
        {
            return null;
        }

        var (applicationId, requestVendorInformation, requestVendorAddress, requestContactInformation, requestPaymentInformation, status) = request;
        var (vendorInformationId, legalName, abn, organisationType, isSmallMediumEnterprise, isIndigenousSupplier) =
            requestVendorInformation ?? new PatchVendorInformation(null, null, null, null, null, null);
        var (addressLine1, addressLine2, addressLine3, city, state, postCode) =
            requestVendorAddress ?? new PatchAddress(null, null, null, null, null, null);
        var contactInformation = requestContactInformation ?? new PatchContactInformation(null);
        contactInformation.Deconstruct(out var email);
        var (accountName, bsb, accountNumber) =
            requestPaymentInformation ?? new PatchPaymentInformation(null, null, null);

        document.ApplicationId = applicationId ?? document.ApplicationId;
        document.VendorInformation = new VendorInformation(
            vendorInformationId ?? document.VendorInformation.Id,
            legalName ?? document.VendorInformation.LegalName,
            abn ?? document.VendorInformation.Abn,
            organisationType ?? document.VendorInformation.OrganisationType,
            isSmallMediumEnterprise ?? document.VendorInformation.IsSmallMediumEnterprise,
            isIndigenousSupplier ?? document.VendorInformation.IsIndigenousSupplier,
            status ?? document.VendorInformation.Status);
        document.VendorAddress = new Address(
            addressLine1 ?? document.VendorAddress.AddressLine1,
            addressLine2 ?? document.VendorAddress.AddressLine2,
            addressLine3 ?? document.VendorAddress.AddressLine3,
            city ?? document.VendorAddress.City,
            state ?? document.VendorAddress.State,
            postCode ?? document.VendorAddress.PostCode);
        document.PaymentInformation = new PaymentInformation(
            accountName ?? document.PaymentInformation.AccountName,
            bsb ?? document.PaymentInformation.BSB,
            accountNumber ?? document.PaymentInformation.AccountNumber);
        document.ContactInformation = new ContactInformation(
            email ?? document.ContactInformation.Email);

        await dbContext.SaveChangesAsync(cancellationToken);
        return document;
    }
}
