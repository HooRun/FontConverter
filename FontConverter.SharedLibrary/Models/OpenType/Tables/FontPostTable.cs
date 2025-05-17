namespace FontConverter.SharedLibrary.Models;

public class FontPostTable
{
    public FontPostTable()
    {
        GlyphNameIndex = new();
        PascalStrings = new();
        GlyphOffsets = new();
    }

    public double Version { get; set; }             
    public double ItalicAngle { get; set; }        
    public short UnderlinePosition { get; set; }
    public short UnderlineThickness { get; set; }
    public uint IsFixedPitch { get; set; }
    public uint MinMemType42 { get; set; }
    public uint MaxMemType42 { get; set; }
    public uint MinMemType1 { get; set; }
    public uint MaxMemType1 { get; set; }

    // Version 2.0
    public List<ushort> GlyphNameIndex { get; set; } 
    public List<string> PascalStrings { get; set; }

    // Version 2.5
    public List<float> GlyphOffsets { get; set; }

}
