using FontConverter.SharedLibrary.Models;
using static FontConverter.SharedLibrary.Helpers.FontTableValueConverterHelper;

namespace FontConverter.SharedLibrary.Helpers;

public static class ParseLocaTableHelper
{
    const int chunkSize = 500;
    public static async Task<FontLocaTable> ParseLocaTable(OpenTypeTableBinaryData tableBinaryData, int numGlyphs, short locFormat, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        FontLocaTable locaTable = new()
        {
            GlyphOffsets = new List<uint>(numGlyphs + 1)
        };

        using var ms = new MemoryStream(tableBinaryData.RawData);
        using var reader = new BinaryReader(ms);

        int count = numGlyphs + 1;


        if (locFormat == 0)
        {
            for (int i = 0; i < count; i += chunkSize)
            {
                cancellationToken.ThrowIfCancellationRequested();
                int batchEnd = Math.Min(i + chunkSize, count);
                for (int j = i; j < batchEnd; j++)
                {
                    ushort val = ReadUInt16BigEndian(reader);
                    locaTable.GlyphOffsets.Add((uint)(val * 2)); // short format: offsets in units of 2 bytes
                }
                await Task.Delay(1).ConfigureAwait(false);
            }
        }
        else
        {
            for (int i = 0; i < count; i += chunkSize)
            {
                cancellationToken.ThrowIfCancellationRequested();
                int batchEnd = Math.Min(i + chunkSize, count);
                for (int j = i; j < batchEnd; j++)
                {
                    uint val = ReadUInt32BigEndian(reader);
                    locaTable.GlyphOffsets.Add(val); // long format: offsets in bytes
                }
                await Task.Delay(1).ConfigureAwait(false);
            }
        }

        return locaTable;
    }
}