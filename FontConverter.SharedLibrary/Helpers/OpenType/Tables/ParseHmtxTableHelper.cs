using FontConverter.SharedLibrary.Models;
using static FontConverter.SharedLibrary.Helpers.FontTableValueConverterHelper;

namespace FontConverter.SharedLibrary.Helpers;

public static class ParseHmtxTableHelper
{
    const int chunkSize = 500;

    public static async Task<FontHmtxTable> ParseHmtxTable(OpenTypeTableBinaryData tableBinaryData, int numGlyphs, int numberOfHMetrics, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        FontHmtxTable hmtxTable = new()
        {
            GlyphMetrics = new List<GlyphHorizontalMetric>(numGlyphs)
        };

        using var ms = new MemoryStream(tableBinaryData.RawData);
        using var reader = new BinaryReader(ms);

        ushort lastAdvanceWidth = 0;


        // Full metrics
        for (int i = 0; i < numberOfHMetrics; i += chunkSize)
        {
            cancellationToken.ThrowIfCancellationRequested();
            int batchEnd = Math.Min(i + chunkSize, numberOfHMetrics);
            for (int j = i; j < batchEnd; j++)
            {
                ushort advanceWidth = ReadUInt16BigEndian(reader);
                short lsb = ReadInt16BigEndian(reader);
                lastAdvanceWidth = advanceWidth;

                hmtxTable.GlyphMetrics.Add(new GlyphHorizontalMetric
                {
                    AdvanceWidth = advanceWidth,
                    LeftSideBearing = lsb
                });
            }
            await Task.Delay(1).ConfigureAwait(false);
        }

        // Remaining glyphs: only LSB is stored, advanceWidth = lastAdvanceWidth
        for (int i = numberOfHMetrics; i < numGlyphs; i += chunkSize)
        {
            cancellationToken.ThrowIfCancellationRequested();
            int batchEnd = Math.Min(i + chunkSize, numGlyphs);
            for (int j = i; j < batchEnd; j++)
            {
                short lsb = ReadInt16BigEndian(reader);
                hmtxTable.GlyphMetrics.Add(new GlyphHorizontalMetric
                {
                    AdvanceWidth = lastAdvanceWidth,
                    LeftSideBearing = lsb
                });
            }
            await Task.Delay(1).ConfigureAwait(false);
        }


        return hmtxTable;
    }
}