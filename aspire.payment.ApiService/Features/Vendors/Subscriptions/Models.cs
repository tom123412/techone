namespace aspire.payment.ApiService.Features.Vendors;

public record CreateVendorSubscriptionRequest(string CallbackUrl);
public record VendorWebhookSubscription(string Id, string CallbackUrl, DateTimeOffset CreatedAtUtc);
public record VendorCreatedEvent(string EventType, DateTimeOffset OccurredAtUtc, GetVendorResponse Vendor);

public class VendorSubscriptionDocument
{
    public required string Id { get; set; }
    public required string CallbackUrl { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
}
