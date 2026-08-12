using Microsoft.EntityFrameworkCore;

namespace aspire.payment.ApiService.Features.Payments.Create;

internal sealed class PaymentsCosmosDbContext(DbContextOptions<PaymentsCosmosDbContext> options) : DbContext(options)
{
    public const string DatabaseId = "payments";

    public DbSet<PaymentDocument> Payments => Set<PaymentDocument>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PaymentDocument>(entity =>
        {
            entity.ToContainer("payments");
            entity.HasKey(document => document.Id);
            entity.HasPartitionKey(document => document.Id);

            entity.Property(document => document.Id).ToJsonProperty("id");
            entity.Property(document => document.InvoiceDate).ToJsonProperty("invoiceDate");
            entity.Property(document => document.LedgerCode).ToJsonProperty("ledgerCode");
            entity.Property(document => document.AccountNumber).ToJsonProperty("accountNumber");
            entity.Property(document => document.GSTExclusiveAmount).ToJsonProperty("gstExclusiveAmount");
            entity.Property(document => document.GSTAmount).ToJsonProperty("gstAmount");
            entity.Property(document => document.GSTInclusiveAmount).ToJsonProperty("gstInclusiveAmount");
            entity.Property(document => document.InvoiceNarration1).ToJsonProperty("invoiceNarration1");
            entity.Property(document => document.InvoiceNarration2).ToJsonProperty("invoiceNarration2");
            entity.Property(document => document.InvoiceNarration3).ToJsonProperty("invoiceNarration3");
            entity.Property(document => document.BID).ToJsonProperty("bid");
            entity.Property(document => document.PurchaseLocation).ToJsonProperty("purchaseLocation");
            entity.Property(document => document.PurchaseOrderNumber).ToJsonProperty("purchaseOrderNumber");
            entity.Property(document => document.GeneralLedgerCode).ToJsonProperty("generalLedgerCode");
            entity.Property(document => document.RegisteredForGST).ToJsonProperty("registeredForGST");
            entity.Property(document => document.ApplicationID).ToJsonProperty("applicationID");
            entity.Property(document => document.RequestedBy).ToJsonProperty("requestedBy");
            entity.Property(document => document.CreatedAtUtc).ToJsonProperty("createdAtUtc");
        });
    }
}
