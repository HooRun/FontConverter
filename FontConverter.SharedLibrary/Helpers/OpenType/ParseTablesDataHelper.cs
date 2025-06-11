using FontConverter.SharedLibrary.Models;
using System;
using System.Diagnostics.CodeAnalysis;
using static FontConverter.SharedLibrary.Helpers.FontTablesEnumHelper;

namespace FontConverter.SharedLibrary.Helpers;

public static class ParseTablesDataHelper
{

    public static async Task<OpenTypeFont> ParseTablesAsync(SortedList<OpenTypeTables, OpenTypeTableBinaryData> tables, [AllowNull] IProgress<(string tableName, double percentage)> progress = null, CancellationToken cancellationToken = default)
    {
        await Task.Delay(1);
        OpenTypeFont fontData = new();
        if (tables == null || tables.Count == 0)
            return fontData;

        fontData.Tables = tables;

        var tasks = new List<Task>();
        object reportLock = new object();
        int completedTasks = 0;
        int totalTasks = 0;

        var expectedTables = new[]
        {
            OpenTypeTables.NAME,
            OpenTypeTables.HEAD,
            OpenTypeTables.OS2,
            OpenTypeTables.MAXP,
            OpenTypeTables.HHEA,
            OpenTypeTables.VHEA,
            OpenTypeTables.KERN,
            OpenTypeTables.CMAP,
            OpenTypeTables.LOCA,
            OpenTypeTables.POST,
            OpenTypeTables.GLYF,
            OpenTypeTables.HMTX,
            OpenTypeTables.VMTX
        };

        // Count needed tasks
        totalTasks = expectedTables.Count(tag => tables.ContainsKey(tag));

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Phase 1: Parse independent tables
            var independentTasks = new List<Task>();
            if (fontData.Tables.TryGetValue(OpenTypeTables.NAME, out OpenTypeTableBinaryData? nameTable) && nameTable != null)
            {
                independentTasks.Add(Task.Run(async () =>
                {
                    fontData.NameTable = await ParseNameTableHelper.ParseNameTable(nameTable, cancellationToken);
                    lock (reportLock)
                    {
                        double percentage = Interlocked.Increment(ref completedTasks) / (double)totalTasks * 100;
                        progress?.Report((nameTable.TagName, percentage));
                    }
                }, cancellationToken));
            }
            if (fontData.Tables.TryGetValue(OpenTypeTables.HEAD, out OpenTypeTableBinaryData? headTable) && headTable != null)
            {
                independentTasks.Add(Task.Run(async () =>
                {
                    fontData.HeadTable = await ParseHeaderTableHelper.ParseHeaderTable(headTable, cancellationToken);
                    lock (reportLock)
                    {
                        double percentage = Interlocked.Increment(ref completedTasks) / (double)totalTasks * 100;
                        progress?.Report((headTable.TagName, percentage));
                    }
                }, cancellationToken));
            }
            if (fontData.Tables.TryGetValue(OpenTypeTables.OS2, out OpenTypeTableBinaryData? os2Table) && os2Table != null)
            {
                independentTasks.Add(Task.Run(async () =>
                {
                    fontData.OS2Table = await ParseOS2TableHelper.ParseOS2Table(os2Table, cancellationToken);
                    lock (reportLock)
                    {
                        double percentage = Interlocked.Increment(ref completedTasks) / (double)totalTasks * 100;
                        progress?.Report((os2Table.TagName, percentage));
                    }
                }, cancellationToken));
            }
            if (fontData.Tables.TryGetValue(OpenTypeTables.MAXP, out OpenTypeTableBinaryData? maxpTable) && maxpTable != null)
            {
                independentTasks.Add(Task.Run(async () =>
                {
                    fontData.MaxpTable = await ParseMaxpTableHelper.ParseMaxpTable(maxpTable, cancellationToken);
                    lock (reportLock)
                    {
                        double percentage = Interlocked.Increment(ref completedTasks) / (double)totalTasks * 100;
                        progress?.Report((maxpTable.TagName, percentage));
                    }
                }, cancellationToken));
            }
            if (fontData.Tables.TryGetValue(OpenTypeTables.HHEA, out OpenTypeTableBinaryData? hheaTable) && hheaTable != null)
            {
                independentTasks.Add(Task.Run(async () =>
                {
                    fontData.HheaTable = await ParseHheaTableHelper.ParseHheaTable(hheaTable, cancellationToken);
                    lock (reportLock)
                    {
                        double percentage = Interlocked.Increment(ref completedTasks) / (double)totalTasks * 100;
                        progress?.Report((hheaTable.TagName, percentage));
                    }
                }, cancellationToken));
            }
            if (fontData.Tables.TryGetValue(OpenTypeTables.VHEA, out OpenTypeTableBinaryData? vheaTable) && vheaTable != null)
            {
                independentTasks.Add(Task.Run(async () =>
                {
                    fontData.VheaTable = await ParseVheaTableHelper.ParseVheaTable(vheaTable, cancellationToken);
                    lock (reportLock)
                    {
                        double percentage = Interlocked.Increment(ref completedTasks) / (double)totalTasks * 100;
                        progress?.Report((vheaTable.TagName, percentage));
                    }
                }, cancellationToken));
            }
            if (fontData.Tables.TryGetValue(OpenTypeTables.KERN, out OpenTypeTableBinaryData? kernTable) && kernTable != null)
            {
                independentTasks.Add(Task.Run(async () =>
                {
                    fontData.KernTable = await ParseKernTableHelper.ParseKernTable(kernTable, cancellationToken);
                    lock (reportLock)
                    {
                        double percentage = Interlocked.Increment(ref completedTasks) / (double)totalTasks * 100;
                        progress?.Report((kernTable.TagName, percentage));
                    }
                }, cancellationToken));
            }
            if (fontData.Tables.TryGetValue(OpenTypeTables.CMAP, out OpenTypeTableBinaryData? cmapTable) && cmapTable != null)
            {
                independentTasks.Add(Task.Run(async () =>
                {
                    fontData.CmapTable = await ParseCmapTableHelper.ParseCmapTable(cmapTable, cancellationToken);
                    lock (reportLock)
                    {
                        double percentage = Interlocked.Increment(ref completedTasks) / (double)totalTasks * 100;
                        progress?.Report((cmapTable.TagName, percentage));
                    }
                }, cancellationToken));
            }

            await Task.WhenAll(independentTasks);
            independentTasks.Clear();

            // Phase 2: Parse LOCA table (depends on MAXP and HEAD)
            var locaTasks = new List<Task>();
            if (fontData.Tables.TryGetValue(OpenTypeTables.LOCA, out OpenTypeTableBinaryData? locaTable) && locaTable != null)
            {
                locaTasks.Add(Task.Run(async () =>
                {
                    fontData.LocaTable = await ParseLocaTableHelper.ParseLocaTable(locaTable, fontData.MaxpTable.NumGlyphs, fontData.HeadTable.IndexToLocFormat, cancellationToken);
                    lock (reportLock)
                    {
                        double percentage = Interlocked.Increment(ref completedTasks) / (double)totalTasks * 100;
                        progress?.Report((locaTable.TagName, percentage));
                    }
                }, cancellationToken));
            }

            await Task.WhenAll(locaTasks);
            locaTasks.Clear();

            // Phase 3: Parse dependent tables (POST, GLYF, HMTX, VMTX)
            var dependentTasks = new List<Task>();
            if (fontData.Tables.TryGetValue(OpenTypeTables.POST, out OpenTypeTableBinaryData? postTable) && postTable != null)
            {
                dependentTasks.Add(Task.Run(async () =>
                {
                    fontData.PostTable = await ParsePostTableHelper.ParsePostTable(postTable, fontData.MaxpTable.NumGlyphs, cancellationToken);
                    lock (reportLock)
                    {
                        double percentage = Interlocked.Increment(ref completedTasks) / (double)totalTasks * 100;
                        progress?.Report((postTable.TagName, percentage));
                    }
                }, cancellationToken));
            }
            if (fontData.Tables.TryGetValue(OpenTypeTables.GLYF, out OpenTypeTableBinaryData? glyfTable) && glyfTable != null)
            {
                dependentTasks.Add(Task.Run(async () =>
                {
                    fontData.GlyfTable = await ParseGlyfTableHelper.ParseGlyfTable(glyfTable, fontData.LocaTable.GlyphOffsets);
                    lock (reportLock)
                    {
                        double percentage = Interlocked.Increment(ref completedTasks) / (double)totalTasks * 100;
                        progress?.Report((glyfTable.TagName, percentage));
                    }
                }, cancellationToken));
            }
            if (fontData.Tables.TryGetValue(OpenTypeTables.HMTX, out OpenTypeTableBinaryData? hmtxTable) && hmtxTable != null)
            {
                dependentTasks.Add(Task.Run(async () =>
                {
                    fontData.HmtxTable = await ParseHmtxTableHelper.ParseHmtxTable(hmtxTable, fontData.MaxpTable.NumGlyphs, fontData.HheaTable.NumberOfHMetrics, cancellationToken);
                    lock (reportLock)
                    {
                        double percentage = Interlocked.Increment(ref completedTasks) / (double)totalTasks * 100;
                        progress?.Report((hmtxTable.TagName, percentage));
                    }
                }, cancellationToken));
            }
            if (fontData.Tables.TryGetValue(OpenTypeTables.VMTX, out OpenTypeTableBinaryData? vmtxTable) && vmtxTable != null)
            {
                dependentTasks.Add(Task.Run(async () =>
                {
                    fontData.VmtxTable = await ParseVmtxTableHelper.ParseVmtxTable(vmtxTable, fontData.MaxpTable.NumGlyphs, fontData.VheaTable.NumberOfLongVerMetrics, cancellationToken);
                    lock (reportLock)
                    {
                        double percentage = Interlocked.Increment(ref completedTasks) / (double)totalTasks * 100;
                        progress?.Report((vmtxTable.TagName, percentage));
                    }
                }, cancellationToken));
            }

            await Task.WhenAll(dependentTasks);
            dependentTasks.Clear();
            return fontData;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw;
        }
        finally
        {
        }
    }
}