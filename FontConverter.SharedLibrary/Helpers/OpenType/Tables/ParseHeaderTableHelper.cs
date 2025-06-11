using FontConverter.SharedLibrary.Models;
using static FontConverter.SharedLibrary.Helpers.FontTablesEnumHelper;
using static FontConverter.SharedLibrary.Helpers.FontTableValueConverterHelper;

namespace FontConverter.SharedLibrary.Helpers;

public static class ParseHeaderTableHelper
{
    public static async Task<FontHeadTable> ParseHeaderTable(OpenTypeTableBinaryData tableBinaryData, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        FontHeadTable headTable = new();

        using var ms = new MemoryStream(tableBinaryData.RawData);
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

        await Task.Delay(1, cancellationToken);
        return headTable;
    }
}