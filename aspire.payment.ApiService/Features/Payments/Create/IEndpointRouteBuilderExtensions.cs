using Asp.Versioning;

namespace aspire.payment.ApiService.Features.Payments.Create;

public static class IEndpointRouteBuilderExtensions
{
    extension(IEndpointRouteBuilder app)
    {
        public IEndpointRouteBuilder MapCreatePaymentEndpoint()
        {
            var apiVersionSet = app.NewApiVersionSet()
                .HasApiVersion(new ApiVersion(1, 0))
                .ReportApiVersions()
                .Build()
                ;

            var paymentsGroup = app
                .MapGroup("/api/payments")
                .WithApiVersionSet(apiVersionSet)
                ;

            paymentsGroup
                .MapPost("/", async (CreatePaymentRequest request, IPaymentStore store, CancellationToken cancellationToken) =>
                {
                    var document = await store.SaveAsync(request, cancellationToken);
                    return Results.Created($"/api/payments/{document.Id}", document);
                })
                .WithName("CreatePayment")
                .MapToApiVersion(1, 0)
                ;

            return app;
        }
    }
}
