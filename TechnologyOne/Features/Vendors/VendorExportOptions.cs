namespace aspire.payment.TechnologyOne.Features.Vendors;

internal sealed class VendorExportOptions
{
    public const string SectionName = "VendorExport";

    public required string Directory { get; set; }
}
