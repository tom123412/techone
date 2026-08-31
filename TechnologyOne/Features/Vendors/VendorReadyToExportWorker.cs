using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text;

namespace aspire.payment.TechnologyOne.Features.Vendors;

internal sealed class VendorReadyToExportWorker(ILogger<VendorReadyToExportWorker> logger, IHttpClientFactory httpClientFactory, IOptions<VendorOptions> options) : BackgroundService
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

                foreach (var vendor in readyToExportVendors)
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
                    csv.Append(EscapeCsv(vendor.ApplicationId));
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

                await File.WriteAllTextAsync(filePath, csv.ToString(), Encoding.UTF8, stoppingToken);

                foreach (var vendor in readyToExportVendors)
                {
                    var patchResponse = await client.PatchAsJsonAsync(
                        $"/api/vendors/{vendor.Id}",
                        new PatchVendorRequest(null, null, null, null, null, VendorStatus.InProgress),
                        stoppingToken);

                    if (!patchResponse.IsSuccessStatusCode)
                    {
                        logger.LogWarning(
                            "Failed updating vendor {VendorId} to InProgress. StatusCode: {StatusCode}",
                            vendor.Id,
                            patchResponse.StatusCode);
                    }
                }

                logger.LogInformation(
                    "Fetched {VendorCount} vendors with status ReadyForExport, wrote CSV to {FilePath}, and attempted status updates to InProgress",
                    readyToExportVendors.Count,
                    filePath);
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
