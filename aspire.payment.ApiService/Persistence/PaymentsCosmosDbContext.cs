using aspire.payment.ApiService.Features.Payments.Create;
using aspire.payment.ApiService.Features.PurchaseOrderItems.Create;
using aspire.payment.ApiService.Features.SupplierForms.Create;
using Microsoft.EntityFrameworkCore;

namespace aspire.payment.ApiService.Persistence;

internal sealed class PaymentsCosmosDbContext(DbContextOptions<PaymentsCosmosDbContext> options) : DbContext(options)
{
    public const string DatabaseId = "payments";

    public DbSet<PaymentDocument> Payments => Set<PaymentDocument>();
    public DbSet<SupplierFormDocument> SupplierForms => Set<SupplierFormDocument>();
    public DbSet<PurchaseOrderItemDocument> PurchaseOrderItems => Set<PurchaseOrderItemDocument>();

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

        modelBuilder.Entity<SupplierFormDocument>(entity =>
        {
            entity.ToContainer("supplierforms");
            entity.HasKey(document => document.Id);
            entity.HasPartitionKey(document => document.Id);

            entity.Property(document => document.Id).ToJsonProperty("id");
            entity.Property(document => document.ApplicationId).ToJsonProperty("applicationId");
            entity.Property(document => document.CreatedAtUtc).ToJsonProperty("createdAtUtc");

            entity.OwnsOne(document => document.SupplierPartyInformation, party =>
            {
                party.ToJsonProperty("supplierPartyInformation");
                party.Property(value => value.LegalName).ToJsonProperty("legalName");
                party.Property(value => value.Abn).ToJsonProperty("abn");
                party.Property(value => value.IsSmallMediumEnterprise).ToJsonProperty("isSmallMediumEnterprise");
                party.Property(value => value.IsIndigenousSupplier).ToJsonProperty("isIndigenousSupplier");
            });

            entity.OwnsOne(document => document.SupplierAddress, address =>
            {
                address.ToJsonProperty("supplierAddress");
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
                payment.Property(value => value.Email).ToJsonProperty("email");
                payment.Property(value => value.AccountName).ToJsonProperty("accountName");
                payment.Property(value => value.BSB).ToJsonProperty("bsb");
                payment.Property(value => value.AccountNumber).ToJsonProperty("accountNumber");
            });
        });

        modelBuilder.Entity<PurchaseOrderItemDocument>(entity =>
        {
            entity.ToContainer("purchaseorderitems");
            entity.HasKey(document => document.Id);
            entity.HasPartitionKey(document => document.Id);

            entity.Property(document => document.Id).ToJsonProperty("id");
            entity.Property(document => document.PurchasingLocation).ToJsonProperty("purchasingLocation");
            entity.Property(document => document.SublocationCode).ToJsonProperty("sublocationCode");
            entity.Property(document => document.RequisitionNumber).ToJsonProperty("requisitionNumber");
            entity.Property(document => document.ProcessingGroupName).ToJsonProperty("processingGroupName");
            entity.Property(document => document.OrderNumber).ToJsonProperty("orderNumber");
            entity.Property(document => document.Reference).ToJsonProperty("reference");
            entity.Property(document => document.RequisitionComment).ToJsonProperty("requisitionComment");
            entity.Property(document => document.CreatedAtUtc).ToJsonProperty("createdAtUtc");

            entity.OwnsOne(document => document.UserFields, userFields =>
            {
                userFields.ToJsonProperty("userFields");
                userFields.Property(value => value.UserFieldH1).ToJsonProperty("userFieldH1");
                userFields.Property(value => value.UserFieldH2).ToJsonProperty("userFieldH2");
                userFields.Property(value => value.UserFieldH3).ToJsonProperty("userFieldH3");
                userFields.Property(value => value.UserFieldH4).ToJsonProperty("userFieldH4");
                userFields.Property(value => value.UserFieldH5).ToJsonProperty("userFieldH5");
                userFields.Property(value => value.UserFieldH6).ToJsonProperty("userFieldH6");
                userFields.Property(value => value.UserFieldH7).ToJsonProperty("userFieldH7");
                userFields.Property(value => value.UserFieldH8).ToJsonProperty("userFieldH8");
                userFields.Property(value => value.UserFieldH9).ToJsonProperty("userFieldH9");
                userFields.Property(value => value.UserFieldH10).ToJsonProperty("userFieldH10");
                userFields.Property(value => value.UserFieldH11).ToJsonProperty("userFieldH11");
                userFields.Property(value => value.UserFieldH12).ToJsonProperty("userFieldH12");
                userFields.Property(value => value.UserFieldH13).ToJsonProperty("userFieldH13");
                userFields.Property(value => value.UserFieldH14).ToJsonProperty("userFieldH14");
                userFields.Property(value => value.UserFieldH15).ToJsonProperty("userFieldH15");
                userFields.Property(value => value.UserFieldH16).ToJsonProperty("userFieldH16");
                userFields.Property(value => value.UserFieldH17).ToJsonProperty("userFieldH17");
                userFields.Property(value => value.UserFieldH18).ToJsonProperty("userFieldH18");
                userFields.Property(value => value.UserFieldH19).ToJsonProperty("userFieldH19");
                userFields.Property(value => value.UserFieldH20).ToJsonProperty("userFieldH20");
            });

            entity.OwnsOne(document => document.OtherInformation, otherInformation =>
            {
                otherInformation.ToJsonProperty("otherInformation");
                otherInformation.Property(value => value.GOId).ToJsonProperty("goId");
                otherInformation.Property(value => value.SupplierLedgerCode).ToJsonProperty("supplierLedgerCode");
                otherInformation.Property(value => value.SupplierAccountNbri).ToJsonProperty("supplierAccountNbri");
                otherInformation.Property(value => value.Description).ToJsonProperty("description");
                otherInformation.Property(value => value.RequisitionStatus).ToJsonProperty("requisitionStatus");
                otherInformation.Property(value => value.DueDateL).ToJsonProperty("dueDateL");
                otherInformation.Property(value => value.DISSId).ToJsonProperty("dissId");
                otherInformation.Property(value => value.LineId).ToJsonProperty("lineId");
                otherInformation.Property(value => value.Service).ToJsonProperty("service");
                otherInformation.Property(value => value.AmtINC).ToJsonProperty("amtINC");
                otherInformation.Property(value => value.LedgerCode).ToJsonProperty("ledgerCode");
                otherInformation.Property(value => value.AccountNBRI).ToJsonProperty("accountNBRI");
                otherInformation.Property(value => value.VATRateCodeL).ToJsonProperty("vatRateCodeL");
            });
        });
    }
}
