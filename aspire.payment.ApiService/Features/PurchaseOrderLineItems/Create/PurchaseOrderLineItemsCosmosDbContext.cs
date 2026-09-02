using Microsoft.EntityFrameworkCore;

namespace aspire.payment.ApiService.Features.PurchaseOrderLineItems.Create;

internal sealed class PurchaseOrderLineItemsCosmosDbContext(DbContextOptions<PurchaseOrderLineItemsCosmosDbContext> options) : DbContext(options)
{
    public DbSet<PurchaseOrderLineItemDocument> PurchaseOrderLineItems => Set<PurchaseOrderLineItemDocument>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PurchaseOrderLineItemDocument>(entity =>
        {
            entity.ToContainer("purchase-order-line-items");
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
