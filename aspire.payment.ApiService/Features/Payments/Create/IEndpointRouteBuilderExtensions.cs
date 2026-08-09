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
                .MapPost("/", (CreatePaymentRequest request) => Results.Created($"/payments/{request.InvoiceDate:yyyyMMdd}-{request.BID}", request))
                .WithName("CreatePayment")
                .MapToApiVersion(1, 0)
                ;

            return app;
        }
    }
}
