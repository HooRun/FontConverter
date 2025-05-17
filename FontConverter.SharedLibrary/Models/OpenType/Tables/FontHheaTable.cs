namespace FontConverter.SharedLibrary.Models;

public class FontHheaTable
{
    public FontHheaTable()
    {
        
    }

    public double Version { get; set; } // 16.16 Fixed
    public short Ascent { get; set; }
    public short Descent { get; set; }
    public short LineGap { get; set; }
    public ushort AdvanceWidthMax { get; set; }
    public short MinLeftSideBearing { get; set; }
    public short MinRightSideBearing { get; set; }
    public short XMaxExtent { get; set; }
    public short CaretSlopeRise { get; set; }
    public short CaretSlopeRun { get; set; }
    public short CaretOffset { get; set; }

    // Reserved fields – should be set to zero when writing and ignored when reading
    public short Reserved1 { get; set; }
    public short Reserved2 { get; set; }
    public short Reserved3 { get; set; }
    public short Reserved4 { get; set; }

    public short MetricDataFormat { get; set; } // Should be 0
    public ushort NumberOfHMetrics { get; set; }
}
