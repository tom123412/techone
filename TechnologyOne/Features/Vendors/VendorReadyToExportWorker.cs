using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text;

namespace aspire.payment.TechnologyOne.Features.Vendors;

internal sealed class VendorReadyToExportWorker(ILogger<VendorReadyToExportWorker> logger, IHttpClientFactory httpClientFactory,
    IOptions<VendorOptions> options, IOptions<FieldMappingsOptions> fieldMappingsOptions) : BackgroundService
{
    private bool _isRegisteredForVendorEvents;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (!_isRegisteredForVendorEvents)
                {
                    _isRegisteredForVendorEvents = await RegisterForVendorEventsAsync(stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to fetch vendors with status ReadyForExport");
            }

            await Task.Delay(options.Value.PollingInterval, stoppingToken);
        }
    }

    public async Task ProcessVendorCreatedEventAsync(VendorCreatedEventPayload vendorCreatedEvent, CancellationToken cancellationToken)
    {
        if (!string.Equals(vendorCreatedEvent.EventType, "vendor.created", StringComparison.OrdinalIgnoreCase))
        {
            logger.LogWarning("Ignoring unsupported vendor event type {EventType}", vendorCreatedEvent.EventType);
            return;
        }

        var client = httpClientFactory.CreateClient("apiservice");
        await ProcessVendorsAsync(client, [vendorCreatedEvent.Vendor], cancellationToken);
    }

    private async Task<bool> RegisterForVendorEventsAsync(CancellationToken cancellationToken)
    {
        var callbackUrl = options.Value.SubscriptionCallbackUrl;
        if (string.IsNullOrWhiteSpace(callbackUrl))
        {
            logger.LogWarning("Vendor subscription callback URL is empty. Skipping vendor event registration.");
            return false;
        }

        var client = httpClientFactory.CreateClient("apiservice");

        using var subscribeResponse = await client.PostAsJsonAsync(
            "/api/vendors/subscriptions",
            new CreateVendorSubscriptionRequest(callbackUrl),
            cancellationToken);

        if (!subscribeResponse.IsSuccessStatusCode)
        {
            logger.LogWarning("Failed to register vendor event subscription. StatusCode: {StatusCode}", subscribeResponse.StatusCode);
            return false;
        }

        logger.LogInformation("Registered vendor event subscription for callback {CallbackUrl}", callbackUrl);

        var response = await client.GetFromJsonAsync<VendorODataResponse>(
            $"/api/vendors?$filter=Status eq '{VendorStatus.ReadyForExport}'",
            cancellationToken);

        var readyToExportVendors = response?.Value ?? [];
        if (readyToExportVendors.Count > 0)
        {
            await ProcessVendorsAsync(client, readyToExportVendors, cancellationToken);
        }

        return true;
    }

    private async Task ProcessVendorsAsync(HttpClient client, List<VendorPayload> vendors, CancellationToken cancellationToken)
    {
        var readyVendors = vendors.Where(v => v.Status == VendorStatus.ReadyForExport).ToList();
        if (readyVendors.Count == 0)
        {
            return;
        }

        var exportDirectorySetting = string.IsNullOrWhiteSpace(options.Value.Directory)
            ? "exports"
            : options.Value.Directory;
        var exportDirectory = Path.IsPathRooted(exportDirectorySetting)
            ? exportDirectorySetting
            : Path.Combine(AppContext.BaseDirectory, exportDirectorySetting);
        Directory.CreateDirectory($"{exportDirectory}\\Outgoing");

        var fileName = $"bulk_supplier_{DateTimeOffset.UtcNow:yyyy-MM-ddTHHmmss}.csv";
        var filePath = Path.Combine(exportDirectory, fileName);

        var csv = new StringBuilder();
        csv.AppendLine("ACCNAME1,BUSREGNBR,SELNCODE1,SELNCODE5,SELNCODE6,USERFLD2,POSTNAME,ADDR1,ADDR2,ADDR3,CITY,STATE,POSTCODE,EMAILADDR,PAYNAME,BSBCODE,BANKACCT,ENQCOMMENT1,USERFLD10");

        var userFld2Key = fieldMappingsOptions.Value.TechnologyOne["USERFLD2"];

        foreach (var vendor in readyVendors)
        {
            csv.Append(EscapeCsv(vendor.VendorInformation.LegalName));
            csv.Append(',');
            csv.Append(EscapeCsv(vendor.VendorInformation.Abn));
            csv.Append(',');
            csv.Append(EscapeCsv(vendor.VendorInformation.OrganisationType));

            csv.Append(',');
            csv.Append(EscapeCsv(vendor.VendorInformation.IsSmallMediumEnterprise.ToString()));
            csv.Append(',');
            csv.Append(EscapeCsv(vendor.VendorInformation.IsIndigenousSupplier.ToString()));
            csv.Append(',');

            var userFld2 = vendor.Metadata.SingleOrDefault(m => m.Key == userFld2Key)?.Value;

            csv.Append(EscapeCsv(userFld2));
            csv.Append(',');
            csv.Append(EscapeCsv(vendor.VendorInformation.LegalName));
            csv.Append(',');
            csv.Append(EscapeCsv(vendor.VendorAddress.AddressLine1));
            csv.Append(',');
            csv.Append(EscapeCsv(vendor.VendorAddress.AddressLine2));
            csv.Append(',');
            csv.Append(EscapeCsv(vendor.VendorAddress.AddressLine3));
            csv.Append(',');
            csv.Append(EscapeCsv(vendor.VendorAddress.City));
            csv.Append(',');
            csv.Append(EscapeCsv(vendor.VendorAddress.State));
            csv.Append(',');
            csv.Append(EscapeCsv(vendor.VendorAddress.PostCode));
            csv.Append(',');
            csv.Append(EscapeCsv(vendor.ContactInformation.Email));
            csv.Append(',');
            csv.Append(EscapeCsv(vendor.PaymentInformation.AccountName));
            csv.Append(',');
            csv.Append(EscapeCsv(vendor.PaymentInformation.BSB));
            csv.Append(',');
            csv.Append(EscapeCsv(vendor.PaymentInformation.AccountNumber));
            csv.Append(',');
            csv.Append(EscapeCsv(vendor.VendorInformation.OrganisationType));
            csv.Append(',');
            csv.Append(EscapeCsv(vendor.VendorInformation.OrganisationType));
            csv.AppendLine();
        }

        await File.WriteAllTextAsync(filePath, csv.ToString(), Encoding.UTF8, cancellationToken);

        foreach (var vendor in readyVendors)
        {
            using var patchResponse = await client.PatchAsJsonAsync(
                $"/api/vendors/{vendor.Id}",
                new PatchVendorRequest(null, null, null, null, null, VendorStatus.InProgress),
                cancellationToken);

            if (!patchResponse.IsSuccessStatusCode)
            {
                logger.LogWarning(
                    "Failed updating vendor {VendorId} to InProgress. StatusCode: {StatusCode}",
                    vendor.Id,
                    patchResponse.StatusCode);
            }
        }

        logger.LogInformation(
            "Processed {VendorCount} vendor(s), wrote CSV to {FilePath}, and attempted status updates to InProgress",
            readyVendors.Count,
            filePath);
    }

    private static string EscapeCsv(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        var escapedValue = value.Replace("\"", "\"\"");
        return $"\"{escapedValue}\"";
    }
}
