using Asp.Versioning;
using Microsoft.AspNetCore.Http.HttpResults;

namespace aspire.payment.ApiService.Features.Vendors;

internal static class VendorSubscriptionEndpointRouteBuilderExtensions
{
    public static RouteGroupBuilder MapVendorSubscriptionEndpoints(this IEndpointRouteBuilder app)
    {
        var apiVersionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .ReportApiVersions()
            .Build()
            ;

        var vendorGroup = app
            .MapGroup("/api/vendors")
            .WithApiVersionSet(apiVersionSet)
            ;

        vendorGroup
            .MapPost("/{id}/subscriptions", async Task<Results<Created<VendorWebhookSubscription>, NotFound, ValidationProblem>> (string id, CreateVendorSubscriptionRequest request, IVendorStore store, CancellationToken cancellationToken) =>
            {
                if (!Uri.TryCreate(request.CallbackUrl, UriKind.Absolute, out var callbackUri) ||
                    (callbackUri.Scheme != Uri.UriSchemeHttp && callbackUri.Scheme != Uri.UriSchemeHttps))
                {
                    return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                    {
                        [nameof(request.CallbackUrl)] = ["CallbackUrl must be an absolute HTTP or HTTPS URL."]
                    });
                }

                var subscription = await store.SubscribeAsync(id, request, cancellationToken);
                return subscription is null
                    ? TypedResults.NotFound()
                    : TypedResults.Created($"/api/vendors/{id}/subscriptions/{subscription.Id}", subscription);
            })
            .WithName("SubscribeVendor")
            .MapToApiVersion(1, 0)
            ;

        return vendorGroup;
    }
}
