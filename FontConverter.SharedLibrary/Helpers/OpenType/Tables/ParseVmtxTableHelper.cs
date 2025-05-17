using FontConverter.SharedLibrary.Models;
using static FontConverter.SharedLibrary.Helpers.FontTableValueConverterHelper;

namespace FontConverter.SharedLibrary.Helpers;

public static class ParseVmtxTableHelper
{
    public static FontVmtxTable ParseVmtxTable(OpenTypeTableBinaryData tableBinaryData, int numGlyphs, int numOfLongVerMetrics)
    {
        FontVmtxTable vmtxTable = new();

        using var ms = new MemoryStream(tableBinaryData.RawData);
        using var reader = new BinaryReader(ms);

        ushort lastAdvanceHeight = 0;

        for (int i = 0; i < numOfLongVerMetrics; i++)
        {
            ushort advanceHeight = ReadUInt16BigEndian(reader);
            short topSideBearing = ReadInt16BigEndian(reader);
            lastAdvanceHeight = advanceHeight;

            vmtxTable.GlyphMetrics.Add(new GlyphVerticalMetric()
            {
                AdvanceHeight = advanceHeight,
                TopSideBearing = topSideBearing
            });
        }

        for (int i = numOfLongVerMetrics; i < numGlyphs; i++)
        {
            short topSideBearing = ReadInt16BigEndian(reader);
            vmtxTable.GlyphMetrics.Add(new GlyphVerticalMetric()
            {
                AdvanceHeight = lastAdvanceHeight,
                TopSideBearing = topSideBearing
            });
        }

        return vmtxTable;
    }
}
