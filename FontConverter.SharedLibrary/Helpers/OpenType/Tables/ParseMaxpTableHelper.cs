using FontConverter.SharedLibrary.Models;
using static FontConverter.SharedLibrary.Helpers.FontTableValueConverterHelper;

namespace FontConverter.SharedLibrary.Helpers;

public static class ParseMaxpTableHelper
{
    public static async Task<FontMaxpTable> ParseMaxpTable(OpenTypeTableBinaryData tableBinaryData, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        FontMaxpTable maxpTable = new();

        using var ms = new MemoryStream(tableBinaryData.RawData);
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

        await Task.Delay(1, cancellationToken);
        return maxpTable;
    }
}