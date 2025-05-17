using FontConverter.SharedLibrary.Models;
using static FontConverter.SharedLibrary.Helpers.FontTableValueConverterHelper;

namespace FontConverter.SharedLibrary.Helpers;

public static class ParseVheaTableHelper
{
    public static FontVheaTable ParseVheaTable(OpenTypeTableBinaryData tableBinaryData)
    {
        FontVheaTable vheaTable = new();

        using var ms = new MemoryStream(tableBinaryData.RawData);
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

        return vheaTable;
    }
}
