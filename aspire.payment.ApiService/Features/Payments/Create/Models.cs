namespace aspire.payment.ApiService.Features.Payments.Create;

public record CreatePaymentRequest(DateOnly InvoiceDate, string LedgerCode, string AccountNumber, decimal GSTExclusiveAmount, decimal GSTAmount,
    decimal GSTInclusiveAmount, string InvoiceNarration1, string InvoiceNarration2, string InvoiceNarration3, string BID, string PurchaseLocation,
    string PurchaseOrderNumber, string GeneralLedgerCode, bool RegisteredForGST, string ApplicationID, string RequestedBy);

public record PaymentDocument(string Id, DateOnly InvoiceDate, string LedgerCode, string AccountNumber, decimal GSTExclusiveAmount, decimal GSTAmount,
    decimal GSTInclusiveAmount, string InvoiceNarration1, string InvoiceNarration2, string InvoiceNarration3, string BID, string PurchaseLocation,
    string PurchaseOrderNumber, string GeneralLedgerCode, bool RegisteredForGST, string ApplicationID, string RequestedBy, DateTimeOffset CreatedAtUtc);
