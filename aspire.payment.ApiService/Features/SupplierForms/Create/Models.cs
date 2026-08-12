namespace aspire.payment.ApiService.Features.SupplierForms.Create;

public record PaymentInformation(string Email, string AccountName, string BSB, string AccountNumber);
public record Address(string AddressLine1, string? AddressLine2, string? AddressLine3, string City, string State, string PostCode);
public record PartyInformation(string LegalName, string? Abn, bool IsSmallMediumEnterprise, bool IsIndigenousSupplier);

public record CreateSupplierFormRequest(string ApplicationID, PartyInformation SupplierPartyInformation, Address SupplierAddress, 
    PaymentInformation PaymentInformation);


public class SupplierFormDocument
{
    public required string Id { get; set; }
    public required string ApplicationId { get; set; }
    public required PartyInformation SupplierPartyInformation { get; set; }
    public required Address SupplierAddress { get; set; }
    public required PaymentInformation PaymentInformation { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
}
