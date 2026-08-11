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
                .MapPost("/", (CreatePurchaseOrderItemRequest request) => Results.Created($"/purchaseorderitems/{request.RequisitionNumber}-{request.OtherInformation.Description}", request))
                .WithName("CreatePurchaseOrderItem")
                .MapToApiVersion(1, 0)
                ;

            return app;
        }
    }
}
