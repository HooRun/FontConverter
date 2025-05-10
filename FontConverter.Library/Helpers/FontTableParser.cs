using LVGLFontConverter.Library.Models;
using LVGLFontConverter.Library.Models.OpenType;
using SkiaSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static LVGLFontConverter.Library.Helpers.CmapTableFormatParser;
using static LVGLFontConverter.Library.Helpers.FontTablesEnum;
using static LVGLFontConverter.Library.Helpers.FontTableValueConverter;
using static LVGLFontConverter.Library.Helpers.GlyfTableDataParser;
using static LVGLFontConverter.Library.Helpers.KernTableDataParser;

namespace LVGLFontConverter.Library.Helpers;

internal static class FontTableParser
{
    internal static async Task<SortedDictionary<OpenTypeTables, OpenTypeTable>> GetFontTablesAsync(SKTypeface typeface, [AllowNull] IProgress<string> progress = null, CancellationToken cancellationToken = default)
    {
        if (typeface == null)
            throw new ArgumentNullException(nameof(typeface));

        return await Task.Run(async () =>
        {
            var tables = new SortedDictionary<OpenTypeTables, OpenTypeTable>();
            uint[] tableTags = typeface.GetTableTags() ?? Array.Empty<uint>();

            if (tableTags.Length == 0)
                return tables;


            foreach (uint tag in tableTags)
            {
                if (!typeface.TryGetTableData(tag, out byte[] tableData) || tableData.Length == 0)
                    continue;

                if (!Enum.IsDefined(typeof(OpenTypeTables), tag))
                {
                    continue;
                }

                var table = new OpenTypeTable
                {
                    Tag = tag,
                    TagName = GetTableReadableTag(tag),
                    Length = typeface.GetTableSize(tag),
                    RawData = tableData
                };
                tables.Add((OpenTypeTables)tag, table);
                progress?.Report("Processed");
                await Task.Delay(50).ConfigureAwait(false);
            }

            return tables;
        }, cancellationToken);
    }

    internal static async Task ParseTablesAsync(OpenTypeFont fontData, [AllowNull] IProgress<(string tableName, double percentage)> progress = null, CancellationToken cancellationToken = default)
    {
        if (fontData == null)
            throw new ArgumentNullException(nameof(fontData));

        var tasks = new List<Task>();
        object reportLock = new object();
        int completedTasks = 0;
        int totalTasks = 0;

        // محاسبه تعداد کل تسک‌ها
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
            // تعریف متد برای گزارش پیشرفت
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
            if (fontData.Tables.TryGetValue(OpenTypeTables.NAME, out OpenTypeTable nameTable))
            {
                tasks.Add(Task.Run(async () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    ParseNameTable(nameTable, fontData.NameTable);
                    ReportProgress(nameTable.TagName);
                    await Task.Delay(100).ConfigureAwait(false);
                }, cancellationToken));
            }
            if (fontData.Tables.TryGetValue(OpenTypeTables.HEAD, out OpenTypeTable headTable))
            {
                tasks.Add(Task.Run(async () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    ParseHeaderTable(headTable, fontData.HeadTable);
                    ReportProgress(headTable.TagName);
                    await Task.Delay(100).ConfigureAwait(false);
                }, cancellationToken));
            }
            if (fontData.Tables.TryGetValue(OpenTypeTables.OS2, out OpenTypeTable os2Table))
            {
                tasks.Add(Task.Run(async () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    ParseOS2Table(os2Table, fontData.OS2Table);
                    ReportProgress(os2Table.TagName);
                    await Task.Delay(100).ConfigureAwait(false);
                }, cancellationToken));
            }
            if (fontData.Tables.TryGetValue(OpenTypeTables.MAXP, out OpenTypeTable maxpTable))
            {
                tasks.Add(Task.Run(async () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    ParseMaxpTable(maxpTable, fontData.MaxpTable);
                    ReportProgress(maxpTable.TagName);
                    await Task.Delay(100).ConfigureAwait(false);
                }, cancellationToken));
            }
            if (fontData.Tables.TryGetValue(OpenTypeTables.HHEA, out OpenTypeTable hheaTable))
            {
                tasks.Add(Task.Run(async () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    ParseHheaTable(hheaTable, fontData.HheaTable);
                    ReportProgress(hheaTable.TagName);
                    await Task.Delay(100).ConfigureAwait(false);
                }, cancellationToken));
            }
            if (fontData.Tables.TryGetValue(OpenTypeTables.VHEA, out OpenTypeTable vheaTable))
            {
                tasks.Add(Task.Run(async () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    ParseVheaTable(vheaTable, fontData.VheaTable);
                    ReportProgress(vheaTable.TagName);
                    await Task.Delay(100).ConfigureAwait(false);
                }, cancellationToken));
            }
            if (fontData.Tables.TryGetValue(OpenTypeTables.KERN, out OpenTypeTable kernTable))
            {
                tasks.Add(Task.Run(async () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    ParseKernTable(kernTable, fontData.KernTable);
                    ReportProgress(kernTable.TagName);
                    await Task.Delay(100).ConfigureAwait(false);
                }, cancellationToken));
            }
            if (fontData.Tables.TryGetValue(OpenTypeTables.CMAP, out OpenTypeTable cmapTable))
            {
                tasks.Add(Task.Run(async () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    ParseCmapTable(cmapTable, fontData.CmapTable);
                    ReportProgress(cmapTable.TagName);
                    await Task.Delay(100).ConfigureAwait(false);
                }, cancellationToken));
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
            tasks.Clear();

            // Phase 2: Parse LOCA table (depends on MAXP and HEAD)
            if (fontData.Tables.TryGetValue(OpenTypeTables.LOCA, out OpenTypeTable locaTable))
            {
                tasks.Add(Task.Run(async () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    ParseLocaTable(locaTable, fontData.LocaTable, fontData.MaxpTable.NumGlyphs, fontData.HeadTable.IndexToLocFormat);
                    ReportProgress(locaTable.TagName);
                    await Task.Delay(100).ConfigureAwait(false);
                }, cancellationToken));
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
            tasks.Clear();

            // Phase 3: Parse dependent tables (POST, GLYF, HMTX, VMTX)
            if (fontData.Tables.TryGetValue(OpenTypeTables.POST, out OpenTypeTable postTable))
            {
                tasks.Add(Task.Run(async () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    ParsePostTable(postTable, fontData.PostTable, fontData.MaxpTable.NumGlyphs);
                    ReportProgress(postTable.TagName);
                    await Task.Delay(100).ConfigureAwait(false);
                }, cancellationToken));
            }
            if (fontData.Tables.TryGetValue(OpenTypeTables.GLYF, out OpenTypeTable glyfTable))
            {
                tasks.Add(Task.Run(async () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    ParseGlyfTable(glyfTable, fontData.GlyfTable, fontData.LocaTable.GlyphOffsets);
                    ReportProgress(glyfTable.TagName);
                    await Task.Delay(100).ConfigureAwait(false);
                }, cancellationToken));
            }
            if (fontData.Tables.TryGetValue(OpenTypeTables.HMTX, out OpenTypeTable hmtxTable))
            {
                tasks.Add(Task.Run(async () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    ParseHmtxTable(hmtxTable, fontData.HmtxTable, fontData.MaxpTable.NumGlyphs, fontData.HheaTable.NumberOfHMetrics);
                    ReportProgress(hmtxTable.TagName);
                    await Task.Delay(100).ConfigureAwait(false);
                }, cancellationToken));
            }
            if (fontData.Tables.TryGetValue(OpenTypeTables.VMTX, out OpenTypeTable vmtxTable))
            {
                tasks.Add(Task.Run(async () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    ParseVmtxTable(vmtxTable, fontData.VmtxTable, fontData.MaxpTable.NumGlyphs, fontData.VheaTable.NumberOfLongVerMetrics);
                    ReportProgress(vmtxTable.TagName);
                    await Task.Delay(100).ConfigureAwait(false);
                }, cancellationToken));
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
        catch (AggregateException ex)
        {
            var nonCancellationExceptions = ex.InnerExceptions
                .Where(e => !(e is OperationCanceledException))
                .ToList();
            if (nonCancellationExceptions.Any())
                throw new AggregateException(nonCancellationExceptions);
            throw;
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

    internal static void ParseNameTable(OpenTypeTable table, FontNameTable nameTable)
    {
        try
        {
            using var ms = new MemoryStream(table.RawData);
            using var reader = new BinaryReader(ms);

            ushort format = ReadUInt16BigEndian(reader);
            ushort count = ReadUInt16BigEndian(reader);
            ushort stringOffset = ReadUInt16BigEndian(reader);

            var langTags = new List<string>();

            // Read langTags for format 1 (Optional)
            long recordStart = reader.BaseStream.Position;
            if (format == 1)
            {
                reader.BaseStream.Position = recordStart + count * 12;
                ushort langTagCount = ReadUInt16BigEndian(reader);
                for (int i = 0; i < langTagCount; i++)
                {
                    ushort length = ReadUInt16BigEndian(reader);
                    ushort offset = ReadUInt16BigEndian(reader);
                    if (offset + length <= table.RawData.Length - stringOffset)
                    {
                        long pos = reader.BaseStream.Position;
                        reader.BaseStream.Position = stringOffset + offset;
                        var bytes = reader.ReadBytes(length);
                        langTags.Add(System.Text.Encoding.ASCII.GetString(bytes));
                        reader.BaseStream.Position = pos;
                    }
                }
            }

            // Read name records
            reader.BaseStream.Position = recordStart;
            for (int i = 0; i < count; i++)
            {
                ushort platformID = ReadUInt16BigEndian(reader);
                ushort encodingID = ReadUInt16BigEndian(reader);
                ushort languageID = ReadUInt16BigEndian(reader);
                ushort nameID = ReadUInt16BigEndian(reader);
                ushort length = ReadUInt16BigEndian(reader);
                ushort offset = ReadUInt16BigEndian(reader);

                if (platformID > 3 || length == 0 || length >= 256 || offset + length > table.RawData.Length - stringOffset)
                    continue;

                try
                {
                    reader.BaseStream.Position = stringOffset + offset;
                    var stringBytes = reader.ReadBytes(length);

                    string name = platformID switch
                    {
                        0 => Encoding.BigEndianUnicode.GetString(stringBytes),
                        1 => Encoding.ASCII.GetString(stringBytes),
                        3 when encodingID == 1 || encodingID == 10 => Encoding.BigEndianUnicode.GetString(stringBytes),
                        3 => Encoding.UTF8.GetString(stringBytes),
                        _ => null
                    };

                    if (string.IsNullOrWhiteSpace(name)) continue;
                    if (!Enum.IsDefined(typeof(NameType), nameID)) continue;

                    var nameType = (NameType)nameID;

                    // Append lang tag if format 1 and languageID >= 0x8000
                    if (format == 1 && languageID >= 0x8000)
                    {
                        int langIndex = languageID - 0x8000;
                        if (langIndex < langTags.Count)
                            name += $" ({langTags[langIndex]})";
                    }

                    // Fill appropriate field
                    switch (nameType)
                    {
                        case NameType.Copyright: nameTable.Copyright = name; break;
                        case NameType.FontFamily: nameTable.FontFamily = name; break;
                        case NameType.FontSubfamily: nameTable.FontSubfamily = name; break;
                        case NameType.UniqueIdentifier: nameTable.UniqueIdentifier = name; break;
                        case NameType.FullFontName: nameTable.FullFontName = name; break;
                        case NameType.Version: nameTable.Version = name; break;
                        case NameType.PostScriptName: nameTable.PostScriptName = name; break;
                        case NameType.Trademark: nameTable.Trademark = name; break;
                        case NameType.Manufacturer: nameTable.Manufacturer = name; break;
                        case NameType.Designer: nameTable.Designer = name; break;
                        case NameType.Description: nameTable.Description = name; break;
                        case NameType.VendorURL: nameTable.VendorURL = name; break;
                        case NameType.DesignerURL: nameTable.DesignerURL = name; break;
                        case NameType.License: nameTable.License = name; break;
                        case NameType.LicenseURL: nameTable.LicenseURL = name; break;
                        case NameType.PreferredFamily: nameTable.PreferredFamily = name; break;
                        case NameType.PreferredSubfamily: nameTable.PreferredSubfamily = name; break;
                        case NameType.CompatibleFull: nameTable.CompatibleFull = name; break;
                        case NameType.SampleText: nameTable.SampleText = name; break;
                        case NameType.PostScriptCID: nameTable.PostScriptCID = name; break;
                        case NameType.WWSFamily: nameTable.WWSFamily = name; break;
                        case NameType.WWSSubfamily: nameTable.WWSSubfamily = name; break;
                        case NameType.LightBackgroundPalette: nameTable.LightBackgroundPalette = name; break;
                        case NameType.DarkBackgroundPalette: nameTable.DarkBackgroundPalette = name; break;
                        case NameType.VariationsPostScriptPrefix: nameTable.VariationsPostScriptPrefix = name; break;
                    }
                }
                catch { continue; }
            }
        }
        catch { }
    }

    internal static void ParseHeaderTable(OpenTypeTable table, FontHeadTable headTable)
    {
        using var ms = new MemoryStream(table.RawData);
        using var reader = new BinaryReader(ms);

        ushort majorVersion = ReadUInt16BigEndian(reader);
        ushort minorVersion = ReadUInt16BigEndian(reader);
        headTable.Version = FixedToDouble((uint)majorVersion << 16 | minorVersion);
        headTable.FontRevision = FixedToDouble(ReadUInt32BigEndian(reader));
        headTable.ChecksumAdjustment = ReadUInt32BigEndian(reader);
        headTable.MagicNumber = ReadUInt32BigEndian(reader);
        ushort flags = ReadUInt16BigEndian(reader);
        foreach (HeadFlags flag in Enum.GetValues(typeof(HeadFlags)))
        {
            if ((flag & (HeadFlags)flags) == flag)
            {
                headTable.Flags.Add(flag);
            }
        }
        headTable.UnitsPerEm = ReadUInt16BigEndian(reader);
        headTable.Created = ReadLongDateTime(ReadInt64BigEndian(reader));
        headTable.Modified = ReadLongDateTime(ReadInt64BigEndian(reader));
        headTable.XMin = ReadInt16BigEndian(reader);
        headTable.YMin = ReadInt16BigEndian(reader);
        headTable.XMax = ReadInt16BigEndian(reader);
        headTable.YMax = ReadInt16BigEndian(reader);
        ushort macStyle = ReadUInt16BigEndian(reader);
        foreach (MacStyleFlags flag in Enum.GetValues(typeof(MacStyleFlags)))
        {
            if ((flag & (MacStyleFlags)macStyle) == flag)
            {
                headTable.MacStyle.Add(flag);
            }
        }
        headTable.LowestRecPPEM = ReadUInt16BigEndian(reader);
        headTable.FontDirectionHint = (FontDirectionHint)ReadUInt16BigEndian(reader);
        headTable.IndexToLocFormat = ReadInt16BigEndian(reader);
        headTable.GlyphDataFormat = ReadInt16BigEndian(reader);
    }

    internal static void ParseHheaTable(OpenTypeTable table, FontHheaTable hheaTable)
    {
        using var ms = new MemoryStream(table.RawData);
        using var reader = new BinaryReader(ms);

        ushort majorVersion = ReadUInt16BigEndian(reader);
        ushort minorVersion = ReadUInt16BigEndian(reader);
        hheaTable.Version = FixedToDouble((uint)majorVersion << 16 | minorVersion);
        hheaTable.Ascent = ReadInt16BigEndian(reader);
        hheaTable.Descent = ReadInt16BigEndian(reader);
        hheaTable.LineGap = ReadInt16BigEndian(reader);
        hheaTable.AdvanceWidthMax = ReadUInt16BigEndian(reader);
        hheaTable.MinLeftSideBearing = ReadInt16BigEndian(reader);
        hheaTable.MinRightSideBearing = ReadInt16BigEndian(reader);
        hheaTable.XMaxExtent = ReadInt16BigEndian(reader);
        hheaTable.CaretSlopeRise = ReadInt16BigEndian(reader);
        hheaTable.CaretSlopeRun = ReadInt16BigEndian(reader);
        hheaTable.CaretOffset = ReadInt16BigEndian(reader);
        hheaTable.Reserved1 = ReadInt16BigEndian(reader);
        hheaTable.Reserved2 = ReadInt16BigEndian(reader);
        hheaTable.Reserved3 = ReadInt16BigEndian(reader);
        hheaTable.Reserved4 = ReadInt16BigEndian(reader);
        hheaTable.MetricDataFormat = ReadInt16BigEndian(reader);
        hheaTable.NumberOfHMetrics = ReadUInt16BigEndian(reader);
    }

    internal static void ParseOS2Table(OpenTypeTable table, FontOS2Table os2Table)
    {
        using var ms = new MemoryStream(table.RawData);
        using var reader = new BinaryReader(ms);

        os2Table.Version = ReadUInt16BigEndian(reader);
        os2Table.XAvgCharWidth = ReadInt16BigEndian(reader);
        os2Table.UsWeightClass = ReadUInt16BigEndian(reader);
        os2Table.UsWidthClass = ReadUInt16BigEndian(reader);
        ushort fsType = ReadUInt16BigEndian(reader);
        foreach (FsTypeFlags flag in Enum.GetValues(typeof(FsTypeFlags)))
        {
            if ((flag & (FsTypeFlags)fsType) == flag)
            {
                os2Table.FsType.Add(flag);
            }
        }

        os2Table.YSubscriptXSize = ReadInt16BigEndian(reader);
        os2Table.YSubscriptYSize = ReadInt16BigEndian(reader);
        os2Table.YSubscriptXOffset = ReadInt16BigEndian(reader);
        os2Table.YSubscriptYOffset = ReadInt16BigEndian(reader);

        os2Table.YSuperscriptXSize = ReadInt16BigEndian(reader);
        os2Table.YSuperscriptYSize = ReadInt16BigEndian(reader);
        os2Table.YSuperscriptXOffset = ReadInt16BigEndian(reader);
        os2Table.YSuperscriptYOffset = ReadInt16BigEndian(reader);

        os2Table.YStrikeoutSize = ReadInt16BigEndian(reader);
        os2Table.YStrikeoutPosition = ReadInt16BigEndian(reader);
        os2Table.SFamilyClass = ReadInt16BigEndian(reader);


        os2Table.Panose.FamilyType = reader.ReadByte();
        os2Table.Panose.SerifStyle = reader.ReadByte();
        os2Table.Panose.Weight = reader.ReadByte();
        os2Table.Panose.Proportion = reader.ReadByte();
        os2Table.Panose.Contrast = reader.ReadByte();
        os2Table.Panose.StrokeVariation = reader.ReadByte();
        os2Table.Panose.ArmStyle = reader.ReadByte();
        os2Table.Panose.LetterForm = reader.ReadByte();
        os2Table.Panose.Midline = reader.ReadByte();
        os2Table.Panose.XHeight = reader.ReadByte();


        uint unicodeRange1 = ReadUInt32BigEndian(reader);
        foreach (UnicodeRange1 flag in Enum.GetValues(typeof(UnicodeRange1)))
        {
            if ((flag & (UnicodeRange1)unicodeRange1) == flag)
            {
                os2Table.UnicodeRange1.Add(flag);
            }
        }
        uint unicodeRange2 = ReadUInt32BigEndian(reader);
        foreach (UnicodeRange2 flag in Enum.GetValues(typeof(UnicodeRange2)))
        {
            if ((flag & (UnicodeRange2)unicodeRange2) == flag)
            {
                os2Table.UnicodeRange2.Add(flag);
            }
        }
        uint unicodeRange3 = ReadUInt32BigEndian(reader);
        foreach (UnicodeRange3 flag in Enum.GetValues(typeof(UnicodeRange3)))
        {
            if ((flag & (UnicodeRange3)unicodeRange3) == flag)
            {
                os2Table.UnicodeRange3.Add(flag);
            }
        }
        uint unicodeRange4 = ReadUInt32BigEndian(reader);
        foreach (UnicodeRange4 flag in Enum.GetValues(typeof(UnicodeRange4)))
        {
            if ((flag & (UnicodeRange4)unicodeRange4) == flag)
            {
                os2Table.UnicodeRange4.Add(flag);
            }
        }

        os2Table.AchVendID = Encoding.ASCII.GetString(reader.ReadBytes(4));

        os2Table.FsSelection = ReadUInt16BigEndian(reader);
        os2Table.UsFirstCharIndex = ReadUInt16BigEndian(reader);
        os2Table.UsLastCharIndex = ReadUInt16BigEndian(reader);

        os2Table.STypoAscender = ReadInt16BigEndian(reader);
        os2Table.STypoDescender = ReadInt16BigEndian(reader);
        os2Table.STypoLineGap = ReadInt16BigEndian(reader);

        os2Table.UsWinAscent = ReadUInt16BigEndian(reader);
        os2Table.UsWinDescent = ReadUInt16BigEndian(reader);

        if (os2Table.Version >= 1)
        {
            uint codePageRange1 = ReadUInt32BigEndian(reader);
            foreach (CodePageRange1 flag in Enum.GetValues(typeof(CodePageRange1)))
            {
                if ((flag & (CodePageRange1)codePageRange1) == flag)
                {
                    os2Table.CodePageRange1.Add(flag);
                }
            }
            uint codePageRange2 = ReadUInt32BigEndian(reader);
            foreach (CodePageRange2 flag in Enum.GetValues(typeof(CodePageRange2)))
            {
                if ((flag & (CodePageRange2)codePageRange2) == flag)
                {
                    os2Table.CodePageRange2.Add(flag);
                }
            }
        }

        if (os2Table.Version >= 2)
        {
            os2Table.SxHeight = ReadInt16BigEndian(reader);
            os2Table.SCapHeight = ReadInt16BigEndian(reader);
            os2Table.UsDefaultChar = ReadUInt16BigEndian(reader);
            os2Table.UsBreakChar = ReadUInt16BigEndian(reader);
            os2Table.UsMaxContext = ReadUInt16BigEndian(reader);
        }

        if (os2Table.Version >= 5)
        {
            os2Table.UsLowerOpticalPointSize = ReadUInt16BigEndian(reader);
            os2Table.UsUpperOpticalPointSize = ReadUInt16BigEndian(reader);
        }
    }

    internal static void ParseMaxpTable(OpenTypeTable table, FontMaxpTable maxpTable)
    {
        using var ms = new MemoryStream(table.RawData);
        using var reader = new BinaryReader(ms);

        ushort majorVersion = ReadUInt16BigEndian(reader);
        ushort minorVersion = ReadUInt16BigEndian(reader);
        maxpTable.Version = FixedToDouble((uint)majorVersion << 16 | minorVersion);
        maxpTable.NumGlyphs = ReadUInt16BigEndian(reader);
        if (maxpTable.Version == 1.0f)
        {
            maxpTable.MaxPoints = ReadUInt16BigEndian(reader);
            maxpTable.MaxContours = ReadUInt16BigEndian(reader);
            maxpTable.MaxCompositePoints = ReadUInt16BigEndian(reader);
            maxpTable.MaxCompositeContours = ReadUInt16BigEndian(reader);
            maxpTable.MaxZones = ReadUInt16BigEndian(reader);
            maxpTable.MaxTwilightPoints = ReadUInt16BigEndian(reader);
            maxpTable.MaxStorage = ReadUInt16BigEndian(reader);
            maxpTable.MaxFunctionDefs = ReadUInt16BigEndian(reader);
            maxpTable.MaxInstructionDefs = ReadUInt16BigEndian(reader);
            maxpTable.MaxStackElements = ReadUInt16BigEndian(reader);
            maxpTable.MaxSizeOfInstructions = ReadUInt16BigEndian(reader);
            maxpTable.MaxComponentElements = ReadUInt16BigEndian(reader);
            maxpTable.MaxComponentDepth = ReadUInt16BigEndian(reader);
        }
    }

    internal static void ParsePostTable(OpenTypeTable table, FontPostTable postTable, int numGlyphs)
    {
        using var ms = new MemoryStream(table.RawData);
        using var reader = new BinaryReader(ms);

        ushort majorVersion = ReadUInt16BigEndian(reader);
        ushort minorVersion = ReadUInt16BigEndian(reader);
        postTable.Version = FixedToDouble((uint)majorVersion << 16 | minorVersion);


        postTable.ItalicAngle = FixedToDouble(ReadUInt32BigEndian(reader));
        postTable.UnderlinePosition = ReadInt16BigEndian(reader);
        postTable.UnderlineThickness = ReadInt16BigEndian(reader);
        postTable.IsFixedPitch = ReadUInt32BigEndian(reader);
        postTable.MinMemType42 = ReadUInt32BigEndian(reader);
        postTable.MaxMemType42 = ReadUInt32BigEndian(reader);
        postTable.MinMemType1 = ReadUInt32BigEndian(reader);
        postTable.MaxMemType1 = ReadUInt32BigEndian(reader);

        if (postTable.Version == 2.0f)
        {

            ushort numGlyphsInTable = ReadUInt16BigEndian(reader);

            for (int i = 0; i < numGlyphsInTable; i++)
                postTable.GlyphNameIndex.Add(ReadUInt16BigEndian(reader));


            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                byte length = reader.ReadByte();
                byte[] strBytes = reader.ReadBytes(length);
                postTable.PascalStrings.Add(Encoding.ASCII.GetString(strBytes));
            }
        }
        else if (postTable.Version == 2.5f)
        {
            for (int i = 0; i < numGlyphs; i++)
            {

                sbyte offset = reader.ReadSByte();
                postTable.GlyphOffsets.Add(offset);
            }
        }

    }

    internal static void ParseCmapTable(OpenTypeTable table, FontCmapTable cmapTable)
    {
        using var ms = new MemoryStream(table.RawData);
        using var reader = new BinaryReader(ms);

        ushort version = ReadUInt16BigEndian(reader);
        ushort numTables = ReadUInt16BigEndian(reader);
        var encodingRecords = new List<(ushort platformID, ushort encodingID, uint subtableOffset)>();

        for (int i = 0; i < numTables; i++)
        {
            ushort platformID = ReadUInt16BigEndian(reader);
            ushort encodingID = ReadUInt16BigEndian(reader);
            uint subtableOffset = ReadUInt32BigEndian(reader);
            encodingRecords.Add((platformID, encodingID, subtableOffset));
        }

        foreach (var (platformID, encodingID, subtableOffset) in encodingRecords)
        {
            long tableStart = subtableOffset;
            reader.BaseStream.Seek(tableStart, SeekOrigin.Begin);
            ushort format = ReadUInt16BigEndian(reader);
            reader.BaseStream.Seek(tableStart, SeekOrigin.Begin);

            var entries = format switch
            {
                0 => ParseFormat0(reader, tableStart),
                4 => ParseFormat4(reader, tableStart),
                6 => ParseFormat6(reader, tableStart),
                10 => ParseFormat10(reader, tableStart),
                12 => ParseFormat12(reader, tableStart),
                13 => ParseFormat13(reader, tableStart),
                14 => ParseFormat14(reader, tableStart),
                _ => null
            };

            if (entries != null)
            {
                foreach (var (unicode, glyph) in entries)
                {
                    cmapTable.UnicodeToGlyphMap[unicode] = glyph;
                }
            }
        }

    }

    internal static void ParseLocaTable(OpenTypeTable table, FontLocaTable locaTable, int numGlyphs, short locFormat)
    {
        using var ms = new MemoryStream(table.RawData);
        using var reader = new BinaryReader(ms);

        int count = numGlyphs + 1;

        if (locFormat == 0)
        {
            for (int i = 0; i < count; i++)
            {
                ushort val = ReadUInt16BigEndian(reader);
                locaTable.GlyphOffsets.Add((uint)(val * 2)); // short format: offsets in units of 2 bytes
            }
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                uint val = ReadUInt32BigEndian(reader);
                locaTable.GlyphOffsets.Add(val); // long format: offsets in bytes
            }
        }
    }

    internal static void ParseGlyfTable(OpenTypeTable table, FontGlyfTable glyfTable, List<uint> offsets)
    {
        using var ms = new MemoryStream(table.RawData);
        using var reader = new BinaryReader(ms);

        for (int i = 0; i < offsets.Count - 1; i++)
        {
            uint offset = offsets[i];
            uint nextOffset = offsets[i + 1];

            if (offset == nextOffset)
            {
                glyfTable.Glyphs.Add(new Glyph());
                continue;
            }

            reader.BaseStream.Seek(offset, SeekOrigin.Begin);
            short numberOfContours = ReadInt16BigEndian(reader);
            short xMin = ReadInt16BigEndian(reader);
            short yMin = ReadInt16BigEndian(reader);
            short xMax = ReadInt16BigEndian(reader);
            short yMax = ReadInt16BigEndian(reader);

            var glyph = new Glyph
            {
                NumberOfContours = numberOfContours,
                XMin = xMin,
                YMin = yMin,
                XMax = xMax,
                YMax = yMax
            };

            if (numberOfContours >= 0)
            {
                //glyph.Simple = ParseSimpleGlyph(reader, numberOfContours);
            }
            else
            {
                //glyph.Composite = ParseCompositeGlyph(reader);
            }

            glyfTable.Glyphs.Add(glyph);
        }


    }

    internal static void ParseVheaTable(OpenTypeTable table, FontVheaTable vheaTable)
    {
        using var ms = new MemoryStream(table.RawData);
        using var reader = new BinaryReader(ms);

        ushort majorVersion = ReadUInt16BigEndian(reader);
        ushort minorVersion = ReadUInt16BigEndian(reader);
        vheaTable.Version = FixedToDouble((uint)majorVersion << 16 | minorVersion);
        vheaTable.Ascender = ReadInt16BigEndian(reader);
        vheaTable.Descender = ReadInt16BigEndian(reader);
        vheaTable.LineGap = ReadInt16BigEndian(reader);
        vheaTable.AdvanceHeightMax = ReadUInt16BigEndian(reader);
        vheaTable.MinTopSideBearing = ReadInt16BigEndian(reader);
        vheaTable.MinBottomSideBearing = ReadInt16BigEndian(reader);
        vheaTable.YMaxExtent = ReadInt16BigEndian(reader);
        vheaTable.CaretSlopeRise = ReadInt16BigEndian(reader);
        vheaTable.CaretSlopeRun = ReadInt16BigEndian(reader);
        vheaTable.CaretOffset = ReadInt16BigEndian(reader);

        // Reserved fields (4x int16)
        vheaTable.Reserved1 = ReadInt16BigEndian(reader);
        vheaTable.Reserved2 = ReadInt16BigEndian(reader);
        vheaTable.Reserved3 = ReadInt16BigEndian(reader);
        vheaTable.Reserved4 = ReadInt16BigEndian(reader);

        vheaTable.MetricDataFormat = ReadInt16BigEndian(reader);
        vheaTable.NumberOfLongVerMetrics = ReadUInt16BigEndian(reader);
    }

    internal static void ParseHmtxTable(OpenTypeTable table, FontHmtxTable hmtxTable, int numGlyphs, int numberOfHMetrics)
    {
        using var ms = new MemoryStream(table.RawData);
        using var reader = new BinaryReader(ms);

        ushort lastAdvanceWidth = 0;

        // Full metrics
        for (int i = 0; i < numberOfHMetrics; i++)
        {
            ushort advanceWidth = ReadUInt16BigEndian(reader);
            short lsb = ReadInt16BigEndian(reader);
            lastAdvanceWidth = advanceWidth;

            hmtxTable.GlyphMetrics.Add(new GlyphHorizontalMetric()
            {
                AdvanceWidth = advanceWidth,
                LeftSideBearing = lsb
            });
        }

        // Remaining glyphs: only LSB is stored, advanceWidth = lastAdvanceWidth
        for (int i = numberOfHMetrics; i < numGlyphs; i++)
        {
            short lsb = ReadInt16BigEndian(reader);
            hmtxTable.GlyphMetrics.Add(new GlyphHorizontalMetric()
            {
                AdvanceWidth = lastAdvanceWidth,
                LeftSideBearing = lsb
            });
        }
    }

    internal static void ParseVmtxTable(OpenTypeTable table, FontVmtxTable vmtxTable, int numGlyphs, int numOfLongVerMetrics)
    {
        using var ms = new MemoryStream(table.RawData);
        using var reader = new BinaryReader(ms);

        ushort lastAdvanceHeight = 0;

        for (int i = 0; i < numOfLongVerMetrics; i++)
        {
            ushort advanceHeight = ReadUInt16BigEndian(reader);
            short topSideBearing = ReadInt16BigEndian(reader);
            lastAdvanceHeight = advanceHeight;

            vmtxTable.GlyphMetrics.Add(new GlyphVerticalMetric()
            {
                AdvanceHeight = advanceHeight,
                TopSideBearing = topSideBearing
            });
        }

        for (int i = numOfLongVerMetrics; i < numGlyphs; i++)
        {
            short topSideBearing = ReadInt16BigEndian(reader);
            vmtxTable.GlyphMetrics.Add(new GlyphVerticalMetric()
            {
                AdvanceHeight = lastAdvanceHeight,
                TopSideBearing = topSideBearing
            });
        }
    }

    internal static void ParseKernTable(OpenTypeTable table, FontKernTable kernTable)
    {
        using var ms = new MemoryStream(table.RawData);
        using var reader = new BinaryReader(ms);

        ushort version = ReadUInt16BigEndian(reader);
        ushort nTables = ReadUInt16BigEndian(reader);

        for (int i = 0; i < nTables; i++)
        {
            long subtableStart = reader.BaseStream.Position;

            ushort versionOrLength = ReadUInt16BigEndian(reader); // for Mac 0x0000 or Win 0x0001
            ushort length = ReadUInt16BigEndian(reader);
            KernCoverage coverage = (KernCoverage)ReadUInt16BigEndian(reader);
            ushort format = (ushort)(((ushort)coverage & (ushort)KernCoverage.FormatMask) >> 8);

            KernSubtable subtable = null;
            switch (format)
            {
                case 0:
                    subtable = KernParseFormat0(reader, subtableStart);
                    break;
                case 2:
                    subtable = KernParseFormat2(reader, subtableStart);
                    break;
            }

            if (subtable != null)
            {
                subtable.Coverage = coverage;
                subtable.Format = format;
                kernTable.Subtables.Add(subtable);
            }

            reader.BaseStream.Seek(subtableStart + length, SeekOrigin.Begin); // move to next subtable
        }

    }
    
}
