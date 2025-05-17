using FontConverter.SharedLibrary.Models;
using static FontConverter.SharedLibrary.Helpers.FontTableValueConverterHelper;

namespace FontConverter.SharedLibrary.Helpers;

public static class ParseHheaTableHelper
{
    public static FontHheaTable ParseHheaTable(OpenTypeTableBinaryData tableBinaryData)
    {
        FontHheaTable hheaTable = new();
        using var ms = new MemoryStream(tableBinaryData.RawData);
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

        return hheaTable;
    }
}
