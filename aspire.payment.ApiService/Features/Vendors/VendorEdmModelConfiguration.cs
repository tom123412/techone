using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace aspire.payment.ApiService.Features.Vendors;

internal static class VendorEdmModelConfiguration
{
    public static IEdmModel GetEdmModel()
    {
        var builder = new ODataConventionModelBuilder();
        builder.EntitySet<VendorDocument>("vendors");
        return builder.GetEdmModel();
    }
}
