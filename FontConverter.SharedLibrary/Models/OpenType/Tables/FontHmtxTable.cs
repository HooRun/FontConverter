namespace FontConverter.SharedLibrary.Models;

public class FontHmtxTable
{
    public FontHmtxTable()
    {
        
    }

    public List<GlyphHorizontalMetric> GlyphMetrics { get; set; } = new();
}

public class GlyphHorizontalMetric
{
    public ushort AdvanceWidth { get; set; }
    public short LeftSideBearing { get; set; }
}