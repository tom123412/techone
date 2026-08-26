using Asp.Versioning;

namespace aspire.payment.ApiService.Features.SupplierForms;

public static class IEndpointRouteBuilderExtensions
{
    extension(IEndpointRouteBuilder app)
    {
        public IEndpointRouteBuilder MapCreateSupplierFormEndpoint()
        {
            var apiVersionSet = app.NewApiVersionSet()
                .HasApiVersion(new ApiVersion(1, 0))
                .ReportApiVersions()
                .Build()
                ;

            var supplierFormGroup = app
                .MapGroup("/api/supplierforms")
                .WithApiVersionSet(apiVersionSet)
                ;

            supplierFormGroup
                .MapPost("/", async (CreateSupplierFormRequest request, ISupplierFormStore store, CancellationToken cancellationToken) =>
                {
                    var document = await store.SaveAsync(request, cancellationToken);
                    return Results.Created($"/api/supplierforms/{document.Id}", document);
                })
                .WithName("CreateSupplierForm")
                .MapToApiVersion(1, 0)
                ;

            supplierFormGroup
                .MapGet("/{id}", async (string id, ISupplierFormStore store, CancellationToken cancellationToken) =>
                {
                    var document = await store.GetAsync(id, cancellationToken);
                    return document is null ? Results.NotFound() : Results.Ok(document);
                })
                .WithName("GetSupplierForm")
                .MapToApiVersion(1, 0)
                ;

            return app;
        }
    }
}
