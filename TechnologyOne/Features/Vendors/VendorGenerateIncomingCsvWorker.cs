using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;

namespace aspire.payment.TechnologyOne.Features.Vendors;

internal sealed class VendorGenerateIncomingCsvWorker(
    ILogger<VendorGenerateIncomingCsvWorker> logger,
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

                var inProgressVendors = response?.Value ?? [];
                var exportDirectorySetting = string.IsNullOrWhiteSpace(options.Value.Directory)
                    ? "exports"
                    : options.Value.Directory;
                var exportDirectory = Path.IsPathRooted(exportDirectorySetting)
                    ? exportDirectorySetting
                    : Path.Combine(AppContext.BaseDirectory, exportDirectorySetting);
                var incomingDirectory = Path.Combine(exportDirectory, "Incoming");
                Directory.CreateDirectory(incomingDirectory);

                var fileName = $"bulk_supplier_{DateTimeOffset.UtcNow:yyyy-MM-ddTHHmmss}.csv";
                var filePath = Path.Combine(incomingDirectory, fileName);

                var csv = new StringBuilder();
                csv.AppendLine("USERFLD2,ACCNBR,BUSREGNBR,ACCNAME1,ACCNAME2,STATUS,COMMENTS");

                var generatedIds = new HashSet<string>(StringComparer.Ordinal);

                foreach (var vendor in inProgressVendors)
                {
                    var vendorInformationId = CreateVendorInformationId(vendor.Id, generatedIds);

                    csv.Append(EscapeCsv(vendor.ApplicationId));
                    csv.Append(',');
                    csv.Append(EscapeCsv(vendorInformationId));
                    csv.Append(',');
                    csv.Append(EscapeCsv(vendor.VendorInformation.Abn));
                    csv.Append(',');
                    csv.Append(EscapeCsv(vendor.VendorInformation.LegalName));
                    csv.Append(',');
                    csv.Append(EscapeCsv(string.Empty));
                    csv.Append(',');
                    csv.Append(EscapeCsv("Success"));
                    csv.Append(',');
                    csv.Append(EscapeCsv("Created by mock service"));
                    csv.AppendLine();
                }

                await File.WriteAllTextAsync(filePath, csv.ToString(), Encoding.UTF8, stoppingToken);

                logger.LogInformation(
                    "Fetched {VendorCount} vendors with status InProgress and wrote CSV to {FilePath}",
                    inProgressVendors.Count,
                    filePath);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to fetch vendors with status InProgress");
            }

            await Task.Delay(options.Value.PollingInterval, stoppingToken);
        }
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
