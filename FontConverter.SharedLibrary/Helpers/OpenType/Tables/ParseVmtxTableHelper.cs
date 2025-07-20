using FontConverter.SharedLibrary.Models;
using static FontConverter.SharedLibrary.Helpers.FontTableValueConverterHelper;

namespace FontConverter.SharedLibrary.Helpers;

public static class ParseVmtxTableHelper
{

    const int chunkSize = 500;
    public static async Task<FontVmtxTable> ParseVmtxTable(OpenTypeTableBinaryData tableBinaryData, int numGlyphs, int numOfLongVerMetrics, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        FontVmtxTable vmtxTable = new()
        {
            GlyphMetrics = new List<GlyphVerticalMetric>(numGlyphs)
        };

        using var ms = new MemoryStream(tableBinaryData.RawData);
        using var reader = new BinaryReader(ms);

        ushort lastAdvanceHeight = 0;

        // Full metrics
        for (int i = 0; i < numOfLongVerMetrics; i += chunkSize)
        {
            cancellationToken.ThrowIfCancellationRequested();
            int batchEnd = Math.Min(i + chunkSize, numOfLongVerMetrics);
            for (int j = i; j < batchEnd; j++)
            {
                ushort advanceHeight = ReadUInt16BigEndian(reader);
                short topSideBearing = ReadInt16BigEndian(reader);
                lastAdvanceHeight = advanceHeight;

                vmtxTable.GlyphMetrics.Add(new GlyphVerticalMetric
                {
                    AdvanceHeight = advanceHeight,
                    TopSideBearing = topSideBearing
                });
            }
            await Task.Delay(1, cancellationToken);
        }

        // Remaining glyphs: only TopSideBearing is stored, AdvanceHeight = lastAdvanceHeight
        for (int i = numOfLongVerMetrics; i < numGlyphs; i += chunkSize)
        {
            cancellationToken.ThrowIfCancellationRequested();
            int batchEnd = Math.Min(i + chunkSize, numGlyphs);
            for (int j = i; j < batchEnd; j++)
            {
                short topSideBearing = ReadInt16BigEndian(reader);
                vmtxTable.GlyphMetrics.Add(new GlyphVerticalMetric
                {
                    AdvanceHeight = lastAdvanceHeight,
                    TopSideBearing = topSideBearing
                });
            }
            await Task.Delay(1, cancellationToken);
        }

        return vmtxTable;
    }
}