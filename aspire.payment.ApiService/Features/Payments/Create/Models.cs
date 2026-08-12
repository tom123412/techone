namespace aspire.payment.ApiService.Features.Payments.Create;

public record CreatePaymentRequest(DateOnly InvoiceDate, string LedgerCode, string AccountNumber, decimal GSTExclusiveAmount, decimal GSTAmount,
    decimal GSTInclusiveAmount, string InvoiceNarration1, string InvoiceNarration2, string InvoiceNarration3, string BID, string PurchaseLocation,
    string PurchaseOrderNumber, string GeneralLedgerCode, bool RegisteredForGST, string ApplicationID, string RequestedBy);

public class PaymentDocument
{
    public required string Id { get; set; }
    public DateOnly InvoiceDate { get; set; }
    public required string LedgerCode { get; set; }
    public required string AccountNumber { get; set; }
    public decimal GSTExclusiveAmount { get; set; }
    public decimal GSTAmount { get; set; }
    public decimal GSTInclusiveAmount { get; set; }
    public required string InvoiceNarration1 { get; set; }
    public required string InvoiceNarration2 { get; set; }
    public required string InvoiceNarration3 { get; set; }
    public required string BID { get; set; }
    public required string PurchaseLocation { get; set; }
    public required string PurchaseOrderNumber { get; set; }
    public required string GeneralLedgerCode { get; set; }
    public bool RegisteredForGST { get; set; }
    public required string ApplicationID { get; set; }
    public required string RequestedBy { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
}
