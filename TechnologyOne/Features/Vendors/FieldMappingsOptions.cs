namespace aspire.payment.TechnologyOne.Features.Vendors;

internal sealed class FieldMappingsOptions
{
    public const string SectionName = "FieldMappings";
    public required TechnologyOneFieldMappingsOptions TechnologyOne { get; set; }
}

internal sealed class TechnologyOneFieldMappingsOptions : Dictionary<string, string>;
