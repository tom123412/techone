namespace aspire.payment.ApiService.Features.Vendors;

public record CreateVendorSubscriptionRequest(string CallbackUrl);
public record VendorWebhookSubscription(string Id, string CallbackUrl, DateTimeOffset CreatedAtUtc);
