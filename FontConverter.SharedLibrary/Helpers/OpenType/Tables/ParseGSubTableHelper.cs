using FontConverter.SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FontConverter.SharedLibrary.Helpers.FontTablesEnumHelper;
using static FontConverter.SharedLibrary.Helpers.FontTableValueConverterHelper;

namespace FontConverter.SharedLibrary.Helpers;

public static class ParseGSubTableHelper
{
    public static GlyphSubstitutionTable ParseGSubTable(OpenTypeTableBinaryData tableBinaryData)
    {
        using var ms = new MemoryStream(tableBinaryData.RawData);
        using var reader = new BinaryReader(ms);

        var gsubTable = new GlyphSubstitutionTable();

        gsubTable.MajorVersion = ReadUInt16BigEndian(reader);
        gsubTable.MinorVersion = ReadUInt16BigEndian(reader);

        ushort scriptListOffset = ReadUInt16BigEndian(reader);
        ushort featureListOffset = ReadUInt16BigEndian(reader);
        ushort lookupListOffset = ReadUInt16BigEndian(reader);

        if (gsubTable.MajorVersion == 1 && gsubTable.MinorVersion >= 1)
            _ = ReadUInt32BigEndian(reader); // featureVariationsOffset — skip

        long lookupListBase = lookupListOffset;

        reader.BaseStream.Seek(lookupListBase, SeekOrigin.Begin);
        ushort lookupCount = ReadUInt16BigEndian(reader);

        ushort[] lookupOffsets = new ushort[lookupCount];
        for (int i = 0; i < lookupCount; i++)
            lookupOffsets[i] = ReadUInt16BigEndian(reader);

        for (int i = 0; i < lookupCount; i++)
        {
            long lookupStart = lookupListBase + lookupOffsets[i];
            reader.BaseStream.Seek(lookupStart, SeekOrigin.Begin);

            var lookupType = (GlyphSubstitutionType)ReadUInt16BigEndian(reader);
            ushort lookupFlag = ReadUInt16BigEndian(reader);
            ushort subTableCount = ReadUInt16BigEndian(reader);

            ushort[] subtableOffsets = new ushort[subTableCount];
            for (int j = 0; j < subTableCount; j++)
                subtableOffsets[j] = ReadUInt16BigEndian(reader);

            foreach (var subOffset in subtableOffsets)
            {
                long subTableStart = lookupStart + subOffset;
                reader.BaseStream.Seek(subTableStart, SeekOrigin.Begin);

                ushort substFormat = ReadUInt16BigEndian(reader);
                ParseSubTable(gsubTable, lookupType, substFormat, reader, subTableStart);
            }
        }

        return gsubTable;
    }

    private static void ParseSubTable(GlyphSubstitutionTable gsubTable, GlyphSubstitutionType type, ushort format, BinaryReader reader, long subTableStart)
    {
        switch (type)
        {
            case GlyphSubstitutionType.Single:
                ParseSingleSubstitution(gsubTable, format, reader, subTableStart);
                break;
            case GlyphSubstitutionType.Multiple:
                ParseMultipleSubstitution(gsubTable, format, reader, subTableStart);
                break;
            case GlyphSubstitutionType.Alternate:
                ParseAlternateSubstitution(gsubTable, format, reader, subTableStart);
                break;
            case GlyphSubstitutionType.Ligature:
                ParseLigatureSubstitution(gsubTable, format, reader, subTableStart);
                break;
            case GlyphSubstitutionType.Extension:
                ParseExtensionSubstitution(gsubTable, reader, subTableStart);
                break;
        }
    }

    private static void ParseSingleSubstitution(GlyphSubstitutionTable gsubTable, ushort format, BinaryReader reader, long subTableStart)
    {
        if (format == 1)
        {
            ushort coverageOffset = ReadUInt16BigEndian(reader);
            short deltaGlyphID = ReadInt16BigEndian(reader);

            var coverage = ReadCoverageTable(reader, subTableStart + coverageOffset);
            foreach (var gid in coverage)
            {
                gsubTable.Substitutions.Add(new GlyphSubstitution
                {
                    Type = GlyphSubstitutionType.Single,
                    FromGlyphIds = new[] { gid },
                    ToGlyphIds = new[] { (ushort)(gid + deltaGlyphID) }
                });
            }
        }
        else if (format == 2)
        {
            ushort coverageOffset = ReadUInt16BigEndian(reader);
            ushort glyphCount = ReadUInt16BigEndian(reader);

            var coverage = ReadCoverageTable(reader, subTableStart + coverageOffset);
            var substitutes = new ushort[glyphCount];

            for (int i = 0; i < glyphCount; i++)
                substitutes[i] = ReadUInt16BigEndian(reader);

            for (int i = 0; i < glyphCount; i++)
            {
                gsubTable.Substitutions.Add(new GlyphSubstitution
                {
                    Type = GlyphSubstitutionType.Single,
                    FromGlyphIds = new[] { coverage[i] },
                    ToGlyphIds = new[] { substitutes[i] }
                });
            }
        }
    }

    private static void ParseMultipleSubstitution(GlyphSubstitutionTable gsubTable, ushort format, BinaryReader reader, long subTableStart)
    {
        if (format != 1) return;

        ushort coverageOffset = ReadUInt16BigEndian(reader);
        ushort sequenceCount = ReadUInt16BigEndian(reader);

        var coverage = ReadCoverageTable(reader, subTableStart + coverageOffset);
        var sequenceOffsets = new ushort[sequenceCount];
        for (int i = 0; i < sequenceCount; i++)
            sequenceOffsets[i] = ReadUInt16BigEndian(reader);

        for (int i = 0; i < sequenceCount; i++)
        {
            reader.BaseStream.Seek(subTableStart + sequenceOffsets[i], SeekOrigin.Begin);
            ushort glyphCount = ReadUInt16BigEndian(reader);

            var substituteGlyphs = new ushort[glyphCount];
            for (int j = 0; j < glyphCount; j++)
                substituteGlyphs[j] = ReadUInt16BigEndian(reader);

            gsubTable.Substitutions.Add(new GlyphSubstitution
            {
                Type = GlyphSubstitutionType.Multiple,
                FromGlyphIds = new[] { coverage[i] },
                ToGlyphIds = substituteGlyphs
            });
        }
    }

    private static void ParseAlternateSubstitution(GlyphSubstitutionTable gsubTable, ushort format, BinaryReader reader, long subTableStart)
    {
        if (format != 1) return;

        ushort coverageOffset = ReadUInt16BigEndian(reader);
        ushort altSetCount = ReadUInt16BigEndian(reader);

        var coverage = ReadCoverageTable(reader, subTableStart + coverageOffset);
        var altSetOffsets = new ushort[altSetCount];
        for (int i = 0; i < altSetCount; i++)
            altSetOffsets[i] = ReadUInt16BigEndian(reader);

        for (int i = 0; i < altSetCount; i++)
        {
            reader.BaseStream.Seek(subTableStart + altSetOffsets[i], SeekOrigin.Begin);
            ushort glyphCount = ReadUInt16BigEndian(reader);

            var alternates = new ushort[glyphCount];
            for (int j = 0; j < glyphCount; j++)
                alternates[j] = ReadUInt16BigEndian(reader);

            gsubTable.Substitutions.Add(new GlyphSubstitution
            {
                Type = GlyphSubstitutionType.Alternate,
                FromGlyphIds = new[] { coverage[i] },
                ToGlyphIds = alternates
            });
        }
    }

    private static void ParseLigatureSubstitution(GlyphSubstitutionTable gsubTable, ushort format, BinaryReader reader, long subTableStart)
    {
        if (format != 1) return;

        ushort coverageOffset = ReadUInt16BigEndian(reader);
        ushort ligSetCount = ReadUInt16BigEndian(reader);

        var coverage = ReadCoverageTable(reader, subTableStart + coverageOffset);
        var ligSetOffsets = new ushort[ligSetCount];
        for (int i = 0; i < ligSetCount; i++)
            ligSetOffsets[i] = ReadUInt16BigEndian(reader);

        for (int i = 0; i < ligSetCount; i++)
        {
            long ligSetPos = subTableStart + ligSetOffsets[i];
            reader.BaseStream.Seek(ligSetPos, SeekOrigin.Begin);

            ushort ligCount = ReadUInt16BigEndian(reader);
            var ligOffsets = new ushort[ligCount];
            for (int j = 0; j < ligCount; j++)
                ligOffsets[j] = ReadUInt16BigEndian(reader);

            foreach (var ligOffset in ligOffsets)
            {
                reader.BaseStream.Seek(ligSetPos + ligOffset, SeekOrigin.Begin);
                ushort ligGlyph = ReadUInt16BigEndian(reader);
                ushort compCount = ReadUInt16BigEndian(reader);

                var components = new ushort[compCount - 1];
                for (int k = 0; k < components.Length; k++)
                    components[k] = ReadUInt16BigEndian(reader);

                var from = new ushort[compCount];
                from[0] = coverage[i];
                Array.Copy(components, 0, from, 1, components.Length);

                gsubTable.Substitutions.Add(new GlyphSubstitution
                {
                    Type = GlyphSubstitutionType.Ligature,
                    FromGlyphIds = from,
                    ToGlyphIds = new[] { ligGlyph }
                });
            }
        }
    }

    private static void ParseExtensionSubstitution(GlyphSubstitutionTable gsubTable, BinaryReader reader, long subTableStart)
    {
        var format = 1; // always 1 for extension
        ushort extensionLookupType = ReadUInt16BigEndian(reader);
        uint extensionOffset = ReadUInt32BigEndian(reader);

        long actualSubTable = subTableStart + extensionOffset;
        reader.BaseStream.Seek(actualSubTable, SeekOrigin.Begin);

        ushort realFormat = ReadUInt16BigEndian(reader); // first thing in subtable
        ParseSubTable(gsubTable, (GlyphSubstitutionType)extensionLookupType, realFormat, reader, actualSubTable);
    }

    private static ushort[] ReadCoverageTable(BinaryReader reader, long offset)
    {
        long originalPos = reader.BaseStream.Position;
        reader.BaseStream.Seek(offset, SeekOrigin.Begin);

        ushort format = ReadUInt16BigEndian(reader);

        if (format == 1)
        {
            ushort glyphCount = ReadUInt16BigEndian(reader);
            var result = new ushort[glyphCount];
            for (int i = 0; i < glyphCount; i++)
                result[i] = ReadUInt16BigEndian(reader);

            reader.BaseStream.Seek(originalPos, SeekOrigin.Begin);
            return result;
        }
        else if (format == 2)
        {
            ushort rangeCount = ReadUInt16BigEndian(reader);
            var result = new List<ushort>();

            for (int i = 0; i < rangeCount; i++)
            {
                ushort start = ReadUInt16BigEndian(reader);
                ushort end = ReadUInt16BigEndian(reader);
                ushort startCoverageIndex = ReadUInt16BigEndian(reader); // ignored here

                for (ushort gid = start; gid <= end; gid++)
                    result.Add(gid);
            }

            reader.BaseStream.Seek(originalPos, SeekOrigin.Begin);
            return result.ToArray();
        }

        throw new NotSupportedException($"Unsupported Coverage Format: {format}");
    }

    // Util functions
    private static ushort ReadUInt16BigEndian(BinaryReader reader)
    {
        var bytes = reader.ReadBytes(2);
        Array.Reverse(bytes);
        return BitConverter.ToUInt16(bytes, 0);
    }

    private static short ReadInt16BigEndian(BinaryReader reader)
    {
        var bytes = reader.ReadBytes(2);
        Array.Reverse(bytes);
        return BitConverter.ToInt16(bytes, 0);
    }

    private static uint ReadUInt32BigEndian(BinaryReader reader)
    {
        var bytes = reader.ReadBytes(4);
        Array.Reverse(bytes);
        return BitConverter.ToUInt32(bytes, 0);
    }
}

