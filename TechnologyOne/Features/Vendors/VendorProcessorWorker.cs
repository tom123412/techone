using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace aspire.payment.TechnologyOne.Features.Vendors;

internal sealed class VendorProcessorWorker(ILogger<VendorProcessorWorker> logger, IHttpClientFactory httpClientFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var client = httpClientFactory.CreateClient("apiservice");
                var response = await client.GetFromJsonAsync<VendorODataResponse>(
                    "/api/vendors?$filter=VendorInformation/Id eq null",
                    stoppingToken);

                var vendorsWithoutVendorInformationId = response?.Value ?? [];

                logger.LogInformation(
                    "Found {VendorCount} vendors without VendorInformation.Id",
                    vendorsWithoutVendorInformationId.Count);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to read vendors without VendorInformation.Id");
            }

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }

    private record VendorODataResponse(List<VendorPayload> Value);

    private record VendorPayload(string Id, VendorInformationPayload VendorInformation);

    private record VendorInformationPayload(string? Id);
}
