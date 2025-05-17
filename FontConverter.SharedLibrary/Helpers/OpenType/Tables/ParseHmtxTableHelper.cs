using FontConverter.SharedLibrary.Models;
using static FontConverter.SharedLibrary.Helpers.FontTableValueConverterHelper;

namespace FontConverter.SharedLibrary.Helpers;

public static class ParseHmtxTableHelper
{
    public static FontHmtxTable ParseHmtxTable(OpenTypeTableBinaryData tableBinaryData, int numGlyphs, int numberOfHMetrics)
    {
        FontHmtxTable hmtxTable = new();

        using var ms = new MemoryStream(tableBinaryData.RawData);
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

        return hmtxTable;
    }
}
