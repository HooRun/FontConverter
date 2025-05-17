using FontConverter.SharedLibrary.Models;
using static FontConverter.SharedLibrary.Helpers.FontTableValueConverterHelper;

namespace FontConverter.SharedLibrary.Helpers;

public static class ParseLocaTableHelper
{
    public static FontLocaTable ParseLocaTable(OpenTypeTableBinaryData tableBinaryData, int numGlyphs, short locFormat)
    {
        FontLocaTable locaTable = new();

        using var ms = new MemoryStream(tableBinaryData.RawData);
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

        return locaTable;
    }
}
