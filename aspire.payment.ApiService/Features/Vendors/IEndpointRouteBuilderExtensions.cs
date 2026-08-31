using Asp.Versioning;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Query;

namespace aspire.payment.ApiService.Features.Vendors;

public static class IEndpointRouteBuilderExtensions
{
    extension(IEndpointRouteBuilder app)
    {
        public IEndpointRouteBuilder MapVendorEndpoints()
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
                .MapPost("/", async Task<Created<VendorDocument>> (CreateVendorRequest request, IVendorStore store, CancellationToken cancellationToken) =>
                {
                    var document = await store.CreateAsync(request, cancellationToken);
                    return TypedResults.Created($"/api/vendors/{document.Id}", document);
                })
                .WithName("CreateVendor")
                .MapToApiVersion(1, 0)
                ;

            vendorGroup
                .MapGet("/", async (IVendorStore store, CancellationToken cancellationToken) =>
                {
                    var vendors = await store.QueryAsync(cancellationToken);
                    return vendors;
                })
                .AddODataQueryEndpointFilter() // Automatically applies query options over IQueryable
                .WithODataModel(VendorEdmModelConfiguration.GetEdmModel()) // Generates accurate @odata.context metadata
                .WithODataResult()            // Properly serializes the response as OData JSON
                .WithName("GetVendors")
                .MapToApiVersion(1, 0)
                ;

            vendorGroup
                .MapGet("/{id}", async Task<Results<Ok<VendorDocument>, NotFound>> (string id, IVendorStore store, CancellationToken cancellationToken) =>
                {
                    var document = await store.GetAsync(id, cancellationToken);
                    return document is null ? TypedResults.NotFound() : TypedResults.Ok(document);
                })
                .WithName("GetVendor")
                .MapToApiVersion(1, 0)
                ;

            vendorGroup
                .MapPatch("/{id}", async Task<Results<Ok<VendorDocument>, NotFound>> (string id, PatchVendorRequest request, IVendorStore store, CancellationToken cancellationToken) =>
                {
                    var document = await store.PatchAsync(id, request, cancellationToken);
                    return document is null ? TypedResults.NotFound() : TypedResults.Ok(document);
                })
                .WithName("PatchVendor")
                .MapToApiVersion(1, 0)
                ;

            return app;
        }
    }
}
