using static FontConverter.SharedLibrary.Helpers.FontTablesEnumHelper;

namespace FontConverter.SharedLibrary.Models;

public class FontHeadTable
{
    public FontHeadTable()
    {
        Flags = new();
        MacStyle = new();
    }

    // Head Table Information
    public double Version { get; set; }
    public double FontRevision { get; set; }
    public uint ChecksumAdjustment { get; set; }
    public uint MagicNumber { get; set; }
    public List<HeadFlags> Flags { get; set; }
    public ushort UnitsPerEm { get; set; }
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
    public short XMin { get; set; }
    public short YMin { get; set; }
    public short XMax { get; set; }
    public short YMax { get; set; }
    public List<MacStyleFlags> MacStyle { get; set; }
    public ushort LowestRecPPEM { get; set; }
    public FontDirectionHint FontDirectionHint { get; set; }
    public short IndexToLocFormat { get; set; } // 0 for short offsets (Offset16), 1 for long (Offset32).
    public short GlyphDataFormat { get; set; }
}
