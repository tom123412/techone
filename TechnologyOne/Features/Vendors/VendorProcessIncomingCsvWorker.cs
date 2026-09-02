using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Channels;

namespace aspire.payment.TechnologyOne.Features.Vendors;

internal sealed class VendorProcessIncomingCsvWorker(
    ILogger<VendorProcessIncomingCsvWorker> logger,
    IHttpClientFactory httpClientFactory,
    IOptions<VendorOptions> options) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var exportDirectorySetting = string.IsNullOrWhiteSpace(options.Value.Directory)
            ? "exports"
            : options.Value.Directory;
        var exportDirectory = Path.IsPathRooted(exportDirectorySetting)
            ? exportDirectorySetting
            : Path.Combine(AppContext.BaseDirectory, exportDirectorySetting);
        var incomingDirectory = Path.Combine(exportDirectory, "Incoming");
        Directory.CreateDirectory(incomingDirectory);

        var triggerChannel = Channel.CreateUnbounded<string>();

        using var watcher = new FileSystemWatcher(incomingDirectory)
        {
            IncludeSubdirectories = false,
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.Size,
            Filter = "*.csv",
            EnableRaisingEvents = true,
        };

        watcher.Created += (_, e) => OnFolderChanged(e.ChangeType, e.FullPath);
        watcher.Changed += (_, e) => OnFolderChanged(e.ChangeType, e.FullPath);
        watcher.Error += (_, e) => logger.LogError(e.GetException(), "Error while watching incoming folder {IncomingDirectory}", incomingDirectory);

        using var cancellationRegistration = stoppingToken.Register(() => triggerChannel.Writer.TryComplete());

        foreach (var filePath in Directory.EnumerateFiles(incomingDirectory, "*.csv"))
        {
            triggerChannel.Writer.TryWrite(filePath);
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (!await triggerChannel.Reader.WaitToReadAsync(stoppingToken))
                {
                    break;
                }

                var changedFilePaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                while (triggerChannel.Reader.TryRead(out var changedFilePath))
                {
                    changedFilePaths.Add(changedFilePath);
                }

                foreach (var filePath in changedFilePaths)
                {
                    await ProcessIncomingCsvFileAsync(filePath, stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed processing incoming vendor CSV files");
            }
        }

        return;

        void OnFolderChanged(WatcherChangeTypes changeType, string filePath)
        {
            logger.LogInformation(
                "Detected {ChangeType} in incoming folder for file {FilePath}",
                changeType,
                filePath);

            triggerChannel.Writer.TryWrite(filePath);
        }
    }

    private async Task ProcessIncomingCsvFileAsync(string filePath, CancellationToken stoppingToken)
    {
        if (!File.Exists(filePath)) return;

        var records = await ReadIncomingCsvAsync(filePath, stoppingToken);
        if (records.Count == 0)
        {
            logger.LogInformation("No records found in incoming file {FilePath}", filePath);
            return;
        }

        var client = httpClientFactory.CreateClient("apiservice");

        foreach (var record in records)
        {
            if (string.IsNullOrWhiteSpace(record.AccName1) || string.IsNullOrWhiteSpace(record.AccNbr))
            {
                logger.LogWarning("Skipping CSV record in file {FilePath} because ACCNAME1 or ACCNBR is empty", filePath);
                continue;
            }

            var escapedLegalName = record.AccName1.Replace("'", "''", StringComparison.Ordinal);
            var response = await client.GetFromJsonAsync<VendorODataResponse>(
                $"/api/vendors?$filter=VendorInformation/LegalName eq '{escapedLegalName}' and Status eq 'InProgress'",
                stoppingToken);

            var vendor = response?.Value.SingleOrDefault();
            if (vendor is null)
            {
                logger.LogWarning("Vendor not found for LegalName {LegalName} from file {FilePath}", record.AccName1, filePath);
                continue;
            }

            var patchRequest = new PatchVendorRequest(
                null,
                new PatchVendorInformation(record.AccNbr, null, null, null, null, null),
                null,
                null,
                null,
                VendorStatus.Completed);

            var patchResponse = await client.PatchAsJsonAsync($"/api/vendors/{vendor.Id}", patchRequest, stoppingToken);
            if (!patchResponse.IsSuccessStatusCode)
            {
                logger.LogWarning(
                    "Failed patching vendor {VendorId} from file {FilePath}. StatusCode: {StatusCode}",
                    vendor.Id,
                    filePath,
                    patchResponse.StatusCode);
            }
        }

        logger.LogInformation("Processed {RecordCount} records from incoming file {FilePath}", records.Count, filePath);
    }

    private static async Task<List<IncomingVendorCsvRecord>> ReadIncomingCsvAsync(string filePath, CancellationToken stoppingToken)
    {
        await using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var reader = new StreamReader(stream, Encoding.UTF8);

        var header = await reader.ReadLineAsync(stoppingToken);
        if (header is null) return [];

        var records = new List<IncomingVendorCsvRecord>();
        while (true)
        {
            var line = await reader.ReadLineAsync(stoppingToken);
            if (line is null) break;

            if (string.IsNullOrWhiteSpace(line)) continue;

            var columns = ParseCsvLine(line);
            if (columns.Count < 7) continue;

            records.Add(new IncomingVendorCsvRecord(
                columns[0],
                columns[1],
                columns[2],
                columns[3],
                columns[4],
                columns[5],
                columns[6]));
        }

        return records;
    }

    private static List<string> ParseCsvLine(string line)
    {
        var columns = new List<string>();
        var current = new StringBuilder();
        var inQuotes = false;

        for (var i = 0; i < line.Length; i++)
        {
            var ch = line[i];
            if (ch == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    current.Append('"');
                    i++;
                    continue;
                }

                inQuotes = !inQuotes;
                continue;
            }

            if (ch == ',' && !inQuotes)
            {
                columns.Add(current.ToString());
                current.Clear();
                continue;
            }

            current.Append(ch);
        }

        columns.Add(current.ToString());
        return columns;
    }
}

internal record IncomingVendorCsvRecord(
    string UserFld2,
    string AccNbr,
    string BusRegNbr,
    string AccName1,
    string? AccName2,
    string Status,
    string Comments);