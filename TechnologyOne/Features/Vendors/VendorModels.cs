using System.Text.Json.Serialization;

namespace aspire.payment.TechnologyOne.Features.Vendors;

internal record VendorODataResponse([property: JsonPropertyName("value")] List<VendorPayload> Value);

internal record VendorPayload(
    string Id,
    string? ApplicationId,
    VendorInformationPayload VendorInformation,
    VendorAddressPayload VendorAddress,
    VendorContactInformationPayload ContactInformation,
    VendorPaymentInformationPayload PaymentInformation,
    IReadOnlyList<VendorMetadataPayload> Metadata);

internal record VendorInformationPayload(string Status, string LegalName, string? Abn, string OrganisationType, bool IsSmallMediumEnterprise, bool IsIndigenousSupplier);

internal record VendorAddressPayload(string AddressLine1, string? AddressLine2, string? AddressLine3, string City, string State, string PostCode);

internal record VendorContactInformationPayload(string Email);

internal record VendorPaymentInformationPayload(string AccountName, string BSB, string AccountNumber);
internal record VendorMetadataPayload(string Key, string Value);

internal record PatchVendorRequest(
    string? ApplicationID,
    PatchVendorInformation? VendorInformation,
    PatchAddress? VendorAddress,
    PatchContactInformation? ContactInformation,
    PatchPaymentInformation? PaymentInformation,
    VendorStatus? Status);

internal enum VendorStatus
{
    ReadyForExport,
    InProgress,
    Completed,
    Error,
}

internal record PatchVendorInformation(string? Id, string? LegalName, string? Abn, string? OrganisationType, bool? IsSmallMediumEnterprise, bool? IsIndigenousSupplier);

internal record PatchAddress(string? AddressLine1, string? AddressLine2, string? AddressLine3, string? City, string? State, string? PostCode);

internal record PatchContactInformation(string? Email);

internal record PatchPaymentInformation(string? AccountName, string? BSB, string? AccountNumber);
