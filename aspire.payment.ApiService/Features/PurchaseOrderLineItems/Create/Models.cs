namespace aspire.payment.ApiService.Features.PurchaseOrderLineItems.Create;

public record UserFields(string UserFieldH1, string UserFieldH2, string? UserFieldH3, string? UserFieldH4, string UserFieldH5,
    string UserFieldH6, DateOnly UserFieldH7, string UserFieldH8, string? UserFieldH9, DateOnly UserFieldH10, DateOnly UserFieldH11,
    string UserFieldH12, string UserFieldH13, string UserFieldH14, string UserFieldH15, string UserFieldH16, string? UserFieldH17,
    string? UserFieldH18, string? UserFieldH19, string? UserFieldH20);
public record OtherInformation(string GOId, string SupplierLedgerCode, string SupplierAccountNbri, string Description, string RequisitionStatus,
    DateOnly DueDateL, string DISSId, string LineId, string Service, decimal AmtINC, string LedgerCode, string AccountNBRI, string VATRateCodeL);

public record CreatePurchaseOrderLineItemRequest(string PurchasingLocation, string SublocationCode, string RequisitionNumber, string ProcessingGroupName,
    string? OrderNumber, string Reference, string RequisitionComment, UserFields UserFields, OtherInformation OtherInformation);

public class PurchaseOrderLineItemDocument
{
    public required string Id { get; set; }
    public required string PurchasingLocation { get; set; }
    public required string SublocationCode { get; set; }
    public required string RequisitionNumber { get; set; }
    public required string ProcessingGroupName { get; set; }
    public string? OrderNumber { get; set; }
    public required string Reference { get; set; }
    public required string RequisitionComment { get; set; }
    public required UserFields UserFields { get; set; }
    public required OtherInformation OtherInformation { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
}
