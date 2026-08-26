using Asp.Versioning;

namespace aspire.payment.ApiService.Features.PurchaseOrderLineItems.Create;

public static class IEndpointRouteBuilderExtensions
{
    extension(IEndpointRouteBuilder app)
    {
        public IEndpointRouteBuilder MapCreatePurchaseOrderLineItemEndpoint()
        {
            var apiVersionSet = app.NewApiVersionSet()
                .HasApiVersion(new ApiVersion(1, 0))
                .ReportApiVersions()
                .Build()
                ;

            var purchaseOrderLineItemGroup = app
                .MapGroup("/api/purchaseorderlineitems")
                .WithApiVersionSet(apiVersionSet)
                ;

            purchaseOrderLineItemGroup
                .MapPost("/", async (CreatePurchaseOrderLineItemRequest request, IPurchaseOrderLineItemStore store, CancellationToken cancellationToken) =>
                {
                    var document = await store.SaveAsync(request, cancellationToken);
                    return Results.Created($"/api/purchaseorderlineitems/{document.Id}", document);
                })
                .WithName("CreatePurchaseOrderLineItem")
                .MapToApiVersion(1, 0)
                ;

            return app;
        }
    }
}
