namespace FontConverter.SharedLibrary.Models;

public class FontVmtxTable
{
    public FontVmtxTable()
    {
        
    }

    public List<GlyphVerticalMetric> GlyphMetrics { get; set; } = new();
}

public class GlyphVerticalMetric
{
    public ushort AdvanceHeight { get; set; }
    public short TopSideBearing { get; set; }
}