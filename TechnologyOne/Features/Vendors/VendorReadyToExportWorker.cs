using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace aspire.payment.TechnologyOne.Features.Vendors;

internal sealed class VendorReadyToExportWorker(ILogger<VendorReadyToExportWorker> logger, IHttpClientFactory httpClientFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var client = httpClientFactory.CreateClient("apiservice");
                var response = await client.GetFromJsonAsync<VendorODataResponse>(
                    "/api/vendors?$filter=Status eq 'ReadyForExport'",
                    stoppingToken);

                var readyToExportVendors = response?.Value ?? [];

                logger.LogInformation(
                    "Fetched {VendorCount} vendors with status ReadyForExport",
                    readyToExportVendors.Count);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to fetch vendors with status ReadyForExport");
            }

            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }

    private record VendorODataResponse([property: JsonPropertyName("value")] List<VendorPayload> Value);

    private record VendorPayload(string Id);
}
