namespace aspire.payment.ApiService.Features.Vendors;

public record PaymentInformation(string AccountName, string BSB, string AccountNumber);
public record ContactInformation(string Email);
public record Address(string AddressLine1, string? AddressLine2, string? AddressLine3, string City, string State, string PostCode);
public record CreateVendorInformation(string LegalName, string? Abn, string OrganisationType, bool IsSmallMediumEnterprise, bool IsIndigenousSupplier);

public record CreateVendorRequest(string ApplicationID, CreateVendorInformation VendorInformation, Address VendorAddress, 
    ContactInformation ContactInformation, PaymentInformation PaymentInformation);

public record PatchVendorInformation(string? LegalName, string? Abn, string? OrganisationType, bool? IsSmallMediumEnterprise, bool? IsIndigenousSupplier);
public record PatchAddress(string? AddressLine1, string? AddressLine2, string? AddressLine3, string? City, string? State, string? PostCode);
public record PatchContactInformation(string? Email);
public record PatchPaymentInformation(string? AccountName, string? BSB, string? AccountNumber);
public record PatchVendorRequest(string? ApplicationID, PatchVendorInformation? VendorInformation, PatchAddress? VendorAddress,
    PatchContactInformation? ContactInformation, PatchPaymentInformation? PaymentInformation);

public record VendorInformation(string? Id, string LegalName, string? Abn, string OrganisationType, bool IsSmallMediumEnterprise, bool IsIndigenousSupplier);

public class VendorDocument
{
    public required string Id { get; set; }
    public required string ApplicationId { get; set; }
    public required VendorInformation VendorInformation { get; set; }
    public required Address VendorAddress { get; set; }
    public required PaymentInformation PaymentInformation { get; set; }
    public required ContactInformation ContactInformation { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
}
