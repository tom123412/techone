using Asp.Versioning;

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
                .MapPost("/", async (CreateVendorRequest request, IVendorStore store, CancellationToken cancellationToken) =>
                {
                    var document = await store.SaveAsync(request, cancellationToken);
                    return Results.Created($"/api/vendors/{document.Id}", document);
                })
                .WithName("CreateVendor")
                .MapToApiVersion(1, 0)
                ;

            vendorGroup
                .MapGet("/", async (IVendorStore store, CancellationToken cancellationToken) =>
                {
                    var documents = await store.GetAllAsync(cancellationToken);
                    return Results.Ok(documents);
                })
                .WithName("GetVendors")
                .MapToApiVersion(1, 0)
                ;

            vendorGroup
                .MapGet("/{id}", async (string id, IVendorStore store, CancellationToken cancellationToken) =>
                {
                    var document = await store.GetAsync(id, cancellationToken);
                    return document is null ? Results.NotFound() : Results.Ok(document);
                })
                .WithName("GetVendor")
                .MapToApiVersion(1, 0)
                ;

            return app;
        }
    }
}
