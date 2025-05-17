using FontConverter.SharedLibrary.Models;
using System.Diagnostics.CodeAnalysis;
using static FontConverter.SharedLibrary.Helpers.FontTablesEnumHelper;

namespace FontConverter.SharedLibrary.Helpers;

public static class ParseTablesDataHelper
{
    public static async Task<OpenTypeFont> ParseTablesAsync([AllowNull] IProgress<(string tableName, double percentage)> progress = null, CancellationToken cancellationToken = default)
    {
        OpenTypeFont fontData = new();

        var tasks = new List<Task>();
        object reportLock = new object();
        int completedTasks = 0;
        int totalTasks = 0;

        totalTasks += fontData.Tables.ContainsKey(OpenTypeTables.NAME) ? 1 : 0;
        totalTasks += fontData.Tables.ContainsKey(OpenTypeTables.HEAD) ? 1 : 0;
        totalTasks += fontData.Tables.ContainsKey(OpenTypeTables.OS2) ? 1 : 0;
        totalTasks += fontData.Tables.ContainsKey(OpenTypeTables.MAXP) ? 1 : 0;
        totalTasks += fontData.Tables.ContainsKey(OpenTypeTables.HHEA) ? 1 : 0;
        totalTasks += fontData.Tables.ContainsKey(OpenTypeTables.VHEA) ? 1 : 0;
        totalTasks += fontData.Tables.ContainsKey(OpenTypeTables.KERN) ? 1 : 0;
        totalTasks += fontData.Tables.ContainsKey(OpenTypeTables.CMAP) ? 1 : 0;
        totalTasks += fontData.Tables.ContainsKey(OpenTypeTables.LOCA) ? 1 : 0;
        totalTasks += fontData.Tables.ContainsKey(OpenTypeTables.POST) ? 1 : 0;
        totalTasks += fontData.Tables.ContainsKey(OpenTypeTables.GLYF) ? 1 : 0;
        totalTasks += fontData.Tables.ContainsKey(OpenTypeTables.HMTX) ? 1 : 0;
        totalTasks += fontData.Tables.ContainsKey(OpenTypeTables.VMTX) ? 1 : 0;

        try
        {
            void ReportProgress(string tableName)
            {
                lock (reportLock)
                {
                    completedTasks++;
                    if (progress != null)
                    {
                        double percentage = (double)completedTasks / totalTasks * 100;
                        progress.Report((tableName, percentage));
                    }
                }
            }

            // Phase 1: Parse independent tables
            if (fontData.Tables.TryGetValue(OpenTypeTables.NAME, out OpenTypeTableBinaryData? nameTable) && nameTable != null)
            {
                tasks.Add(Task.Run(async () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    fontData.NameTable = ParseNameTableHelper.ParseNameTable(nameTable);
                    ReportProgress(nameTable.TagName);
                    await Task.Delay(100).ConfigureAwait(false);
                }, cancellationToken));
            }
            if (fontData.Tables.TryGetValue(OpenTypeTables.HEAD, out OpenTypeTableBinaryData? headTable) && headTable != null)
            {
                tasks.Add(Task.Run(async () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    fontData.HeadTable = ParseHeaderTableHelper.ParseHeaderTable(headTable);
                    ReportProgress(headTable.TagName);
                    await Task.Delay(100).ConfigureAwait(false);
                }, cancellationToken));
            }
            if (fontData.Tables.TryGetValue(OpenTypeTables.OS2, out OpenTypeTableBinaryData? os2Table) && os2Table != null)
            {
                tasks.Add(Task.Run(async () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    fontData.OS2Table = ParseOS2TableHelper.ParseOS2Table(os2Table);
                    ReportProgress(os2Table.TagName);
                    await Task.Delay(100).ConfigureAwait(false);
                }, cancellationToken));
            }
            if (fontData.Tables.TryGetValue(OpenTypeTables.MAXP, out OpenTypeTableBinaryData? maxpTable) && maxpTable != null)
            {
                tasks.Add(Task.Run(async () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    fontData.MaxpTable = ParseMaxpTableHelper.ParseMaxpTable(maxpTable);
                    ReportProgress(maxpTable.TagName);
                    await Task.Delay(100).ConfigureAwait(false);
                }, cancellationToken));
            }
            if (fontData.Tables.TryGetValue(OpenTypeTables.HHEA, out OpenTypeTableBinaryData? hheaTable) && hheaTable != null)
            {
                tasks.Add(Task.Run(async () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    fontData.HheaTable = ParseHheaTableHelper.ParseHheaTable(hheaTable);
                    ReportProgress(hheaTable.TagName);
                    await Task.Delay(100).ConfigureAwait(false);
                }, cancellationToken));
            }
            if (fontData.Tables.TryGetValue(OpenTypeTables.VHEA, out OpenTypeTableBinaryData? vheaTable) && vheaTable != null)
            {
                tasks.Add(Task.Run(async () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    fontData.VheaTable = ParseVheaTableHelper.ParseVheaTable(vheaTable);
                    ReportProgress(vheaTable.TagName);
                    await Task.Delay(100).ConfigureAwait(false);
                }, cancellationToken));
            }
            if (fontData.Tables.TryGetValue(OpenTypeTables.KERN, out OpenTypeTableBinaryData? kernTable) && kernTable != null)
            {
                tasks.Add(Task.Run(async () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    fontData.KernTable = ParseKernTableHelper.ParseKernTable(kernTable);
                    ReportProgress(kernTable.TagName);
                    await Task.Delay(100).ConfigureAwait(false);
                }, cancellationToken));
            }
            if (fontData.Tables.TryGetValue(OpenTypeTables.CMAP, out OpenTypeTableBinaryData? cmapTable) && cmapTable != null)
            {
                tasks.Add(Task.Run(async () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    fontData.CmapTable = ParseCmapTableHelper.ParseCmapTable(cmapTable);
                    ReportProgress(cmapTable.TagName);
                    await Task.Delay(100).ConfigureAwait(false);
                }, cancellationToken));
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
            tasks.Clear();

            // Phase 2: Parse LOCA table (depends on MAXP and HEAD)
            if (fontData.Tables.TryGetValue(OpenTypeTables.LOCA, out OpenTypeTableBinaryData? locaTable) && locaTable != null)
            {
                tasks.Add(Task.Run(async () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    fontData.LocaTable = ParseLocaTableHelper.ParseLocaTable(locaTable, fontData.MaxpTable.NumGlyphs, fontData.HeadTable.IndexToLocFormat);
                    ReportProgress(locaTable.TagName);
                    await Task.Delay(100).ConfigureAwait(false);
                }, cancellationToken));
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
            tasks.Clear();

            // Phase 3: Parse dependent tables (POST, GLYF, HMTX, VMTX)
            if (fontData.Tables.TryGetValue(OpenTypeTables.POST, out OpenTypeTableBinaryData? postTable) && postTable != null)
            {
                tasks.Add(Task.Run(async () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    fontData.PostTable = ParsePostTableHelper.ParsePostTable(postTable, fontData.MaxpTable.NumGlyphs);
                    ReportProgress(postTable.TagName);
                    await Task.Delay(100).ConfigureAwait(false);
                }, cancellationToken));
            }
            if (fontData.Tables.TryGetValue(OpenTypeTables.GLYF, out OpenTypeTableBinaryData? glyfTable) && glyfTable != null)
            {
                tasks.Add(Task.Run(async () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    fontData.GlyfTable = ParseGlyfTableHelper.ParseGlyfTable(glyfTable, fontData.LocaTable.GlyphOffsets);
                    ReportProgress(glyfTable.TagName);
                    await Task.Delay(100).ConfigureAwait(false);
                }, cancellationToken));
            }
            if (fontData.Tables.TryGetValue(OpenTypeTables.HMTX, out OpenTypeTableBinaryData? hmtxTable) && hmtxTable != null)
            {
                tasks.Add(Task.Run(async () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    fontData.HmtxTable = ParseHmtxTableHelper.ParseHmtxTable(hmtxTable, fontData.MaxpTable.NumGlyphs, fontData.HheaTable.NumberOfHMetrics);
                    ReportProgress(hmtxTable.TagName);
                    await Task.Delay(100).ConfigureAwait(false);
                }, cancellationToken));
            }
            if (fontData.Tables.TryGetValue(OpenTypeTables.VMTX, out OpenTypeTableBinaryData? vmtxTable) && vmtxTable != null)
            {
                tasks.Add(Task.Run(async () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    fontData.VmtxTable = ParseVmtxTableHelper.ParseVmtxTable(vmtxTable, fontData.MaxpTable.NumGlyphs, fontData.VheaTable.NumberOfLongVerMetrics);
                    ReportProgress(vmtxTable.TagName);
                    await Task.Delay(100).ConfigureAwait(false);
                }, cancellationToken));
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);

            return fontData;
        }
        catch (OperationCanceledException)
        {
            throw new OperationCanceledException();
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            tasks.Clear();
        }
    }
}
