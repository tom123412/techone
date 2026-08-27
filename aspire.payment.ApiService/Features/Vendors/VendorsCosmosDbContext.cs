using Microsoft.EntityFrameworkCore;

namespace aspire.payment.ApiService.Features.Vendors;

internal sealed class VendorsCosmosDbContext(DbContextOptions<VendorsCosmosDbContext> options) : DbContext(options)
{
    public const string DatabaseId = "payments";

    public DbSet<VendorDocument> Vendors => Set<VendorDocument>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<VendorDocument>(entity =>
        {
            entity.ToContainer("vendors");
            entity.HasKey(document => document.Id);
            entity.HasPartitionKey(document => document.Id);

            entity.Property(document => document.Id).ToJsonProperty("id");
            entity.Property(document => document.ApplicationId).ToJsonProperty("applicationId");
            entity.Property(document => document.CreatedAtUtc).ToJsonProperty("createdAtUtc");

            entity.OwnsOne(document => document.VendorInformation, party =>
            {
                party.ToJsonProperty("vendorPartyInformation");
                party.Property(value => value.LegalName).ToJsonProperty("legalName");
                party.Property(value => value.Abn).ToJsonProperty("abn");
                party.Property(value => value.IsSmallMediumEnterprise).ToJsonProperty("isSmallMediumEnterprise");
                party.Property(value => value.IsIndigenousSupplier).ToJsonProperty("isIndigenousSupplier");
            });

            entity.OwnsOne(document => document.VendorAddress, address =>
            {
                address.ToJsonProperty("vendorAddress");
                address.Property(value => value.AddressLine1).ToJsonProperty("addressLine1");
                address.Property(value => value.AddressLine2).ToJsonProperty("addressLine2");
                address.Property(value => value.AddressLine3).ToJsonProperty("addressLine3");
                address.Property(value => value.City).ToJsonProperty("city");
                address.Property(value => value.State).ToJsonProperty("state");
                address.Property(value => value.PostCode).ToJsonProperty("postCode");
            });

            entity.OwnsOne(document => document.PaymentInformation, payment =>
            {
                payment.ToJsonProperty("paymentInformation");
                payment.Property(value => value.AccountName).ToJsonProperty("accountName");
                payment.Property(value => value.BSB).ToJsonProperty("bsb");
                payment.Property(value => value.AccountNumber).ToJsonProperty("accountNumber");
            });

            entity.OwnsOne(document => document.ContactInformation, contact =>
            {
                contact.ToJsonProperty("contactInformation");
                contact.Property(value => value.Email).ToJsonProperty("email");
            });
        });
    }
}
