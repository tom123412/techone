using Asp.Versioning;

namespace aspire.payment.ApiService.Features.SupplierForms.Create;

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
                .MapPost("/", (CreateSupplierFormRequest request) => Results.Created($"/supplierforms/{request.SupplierPartyInformation.LegalName}-{request.ApplicationID}", request))
                .WithName("CreateSupplierForm")
                .MapToApiVersion(1, 0)
                ;

            return app;
        }
    }
}
