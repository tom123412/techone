namespace aspire.payment.ApiService.Features.Vendors;

public enum Status
{
    ReadyForExport,
    InProgress,
    Completed,
    Error,
}

public record PaymentInformation(string AccountName, string BSB, string AccountNumber);
public record ContactInformation(string Email);
public record Address(string AddressLine1, string? AddressLine2, string? AddressLine3, string City, string State, string PostCode);
public record CreateVendorInformation(string LegalName, string? Abn, string OrganisationType, bool IsSmallMediumEnterprise, bool IsIndigenousSupplier);
public record Metadata(string Key, string Value);

public record CreateVendorRequest(string ApplicationId, CreateVendorInformation VendorInformation, Address VendorAddress, 
    ContactInformation ContactInformation, PaymentInformation PaymentInformation, IReadOnlyList<Metadata>? Metadata)
{
    public IReadOnlyList<Metadata> Metadata { get; init; } = Metadata ?? [];
}

public record PatchVendorInformation(string? Id, string? LegalName, string? Abn, string? OrganisationType, bool? IsSmallMediumEnterprise, bool? IsIndigenousSupplier);
public record PatchAddress(string? AddressLine1, string? AddressLine2, string? AddressLine3, string? City, string? State, string? PostCode);
public record PatchContactInformation(string? Email);
public record PatchPaymentInformation(string? AccountName, string? BSB, string? AccountNumber);
public record PatchVendorRequest(
    string? ApplicationId,
    PatchVendorInformation? VendorInformation,
    PatchAddress? VendorAddress,
    PatchContactInformation? ContactInformation,
    PatchPaymentInformation? PaymentInformation,
    Status? Status);

public record VendorInformation(string? Id, string LegalName, string? Abn, string OrganisationType, bool IsSmallMediumEnterprise, bool IsIndigenousSupplier);

public class VendorDocument
{
    public required string Id { get; set; }
    public required Status Status { get; set; }
    public required string ApplicationId { get; set; }
    public required VendorInformation VendorInformation { get; set; }
    public required Address VendorAddress { get; set; }
    public required PaymentInformation PaymentInformation { get; set; }
    public required ContactInformation ContactInformation { get; set; }
    public IReadOnlyList<Metadata> Metadata { get; set; } = [];
    public DateTimeOffset CreatedAtUtc { get; set; }
}
