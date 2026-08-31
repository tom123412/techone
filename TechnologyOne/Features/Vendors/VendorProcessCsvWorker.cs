using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace aspire.payment.TechnologyOne.Features.Vendors;

internal sealed class VendorProcessCsvWorker(ILogger<VendorProcessCsvWorker> logger, IHttpClientFactory httpClientFactory) : BackgroundService
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
                    var patchRequest = new PatchVendorRequest(new PatchVendorInformation(vendorInformationId));

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

            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
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

    private record VendorODataResponse([property: JsonPropertyName("value")] List<VendorPayload> Value);

    private record VendorPayload(string Id, VendorInformationPayload VendorInformation);

    private record VendorInformationPayload(string? Id);

    private record PatchVendorRequest(PatchVendorInformation? VendorInformation);

    private record PatchVendorInformation(string Id);
}
