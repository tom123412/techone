using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;

namespace aspire.payment.TechnologyOne.Features.Vendors;

internal sealed class VendorProcessIncomingCsvWorker(
    ILogger<VendorProcessIncomingCsvWorker> logger,
    IHttpClientFactory httpClientFactory,
    IOptions<VendorOptions> options) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var client = httpClientFactory.CreateClient("apiservice");
                var response = await client.GetFromJsonAsync<VendorODataResponse>(
                    "/api/vendors?$filter=Status eq 'InProgress'",
                    stoppingToken);

                var vendorsWithoutVendorInformationId = response?.Value ?? [];
                var generatedIds = new HashSet<string>(StringComparer.Ordinal);

                foreach (var vendor in vendorsWithoutVendorInformationId)
                {
                    var vendorInformationId = CreateVendorInformationId(vendor.Id, generatedIds);
                    var patchRequest = new PatchVendorRequest(
                        null,
                        new PatchVendorInformation(vendorInformationId, null, null, null, null, null),
                        null,
                        null,
                        null,
                        VendorStatus.Completed);

                    var patchResponse = await client.PatchAsJsonAsync($"/api/vendors/{vendor.Id}", patchRequest, stoppingToken);

                    if (!patchResponse.IsSuccessStatusCode)
                    {
                        logger.LogWarning(
                            "Failed patching vendor {VendorId} with VendorInformation.Id {VendorInformationId}. StatusCode: {StatusCode}",
                            vendor.Id,
                            vendorInformationId,
                            patchResponse.StatusCode);
                    }
                }

                logger.LogInformation(
                    "Processed {VendorCount} vendors without VendorInformation.Id",
                    vendorsWithoutVendorInformationId.Count);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process vendors without VendorInformation.Id");
            }

            await Task.Delay(options.Value.PollingInterval, stoppingToken);
        }
    }

    private static string CreateVendorInformationId(string vendorId, HashSet<string> generatedIds)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(vendorId));
        var seed = BitConverter.ToUInt32(hash, 0) % 1_000_000;

        var attempts = 0u;
        while (attempts < 1_000_000)
        {
            var candidate = $"C{((seed + attempts) % 1_000_000):D6}";
            if (generatedIds.Add(candidate))
            {
                return candidate;
            }

            attempts++;
        }

        throw new InvalidOperationException("Unable to generate a unique VendorInformation.Id.");
    }
}
