namespace aspire.payment.ApiService.Features.Vendors;

public record PaymentInformation(string Email, string AccountName, string BSB, string AccountNumber);
public record Address(string AddressLine1, string? AddressLine2, string? AddressLine3, string City, string State, string PostCode);
public record PartyInformation(string LegalName, string? Abn, bool IsSmallMediumEnterprise, bool IsIndigenousSupplier);

public record CreateVendorRequest(string ApplicationID, PartyInformation VendorPartyInformation, Address VendorAddress,
    PaymentInformation PaymentInformation);

public class VendorDocument
{
    public required string Id { get; set; }
    public required string ApplicationId { get; set; }
    public required PartyInformation VendorPartyInformation { get; set; }
    public required Address VendorAddress { get; set; }
    public required PaymentInformation PaymentInformation { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
}
