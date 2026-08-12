using Asp.Versioning;

namespace aspire.payment.ApiService.Features.PurchaseOrderItems.Create;

public static class IEndpointRouteBuilderExtensions
{
    extension(IEndpointRouteBuilder app)
    {
        public IEndpointRouteBuilder MapCreatePurchaseOrderItemEndpoint()
        {
            var apiVersionSet = app.NewApiVersionSet()
                .HasApiVersion(new ApiVersion(1, 0))
                .ReportApiVersions()
                .Build()
                ;

            var purchaseOrderItemGroup = app
                .MapGroup("/api/purchaseorderitems")
                .WithApiVersionSet(apiVersionSet)
                ;

            purchaseOrderItemGroup
                .MapPost("/", async (CreatePurchaseOrderItemRequest request, IPurchaseOrderItemStore store, CancellationToken cancellationToken) =>
                {
                    var document = await store.SaveAsync(request, cancellationToken);
                    return Results.Created($"/api/purchaseorderitems/{document.Id}", document);
                })
                .WithName("CreatePurchaseOrderItem")
                .MapToApiVersion(1, 0)
                ;

            return app;
        }
    }
}
