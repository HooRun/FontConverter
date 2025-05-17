using static FontConverter.SharedLibrary.Helpers.FontTablesEnumHelper;

namespace FontConverter.SharedLibrary.Models;

public class FontGlyfTable
{
    public FontGlyfTable()
    {
        
    }

    public List<Glyph> Glyphs { get; set; } = new();
}

public class Glyph
{
    public short NumberOfContours { get; set; }
    public short XMin { get; set; }
    public short YMin { get; set; }
    public short XMax { get; set; }
    public short YMax { get; set; }
    public SimpleGlyph Simple { get; set; } = new();
    public CompositeGlyph Composite { get; set; } = new();
    public bool IsComposite => NumberOfContours < 0;
}

public class SimpleGlyph
{
    public List<ushort> EndPtsOfContours { get; set; } = new();
    public ushort InstructionLength { get; set; }
    public byte[] Instructions { get; set; } = [];
    public List<GlyphPoint> Points { get; set; } = new();
}

public class GlyphPoint
{
    public short X { get; set; }
    public short Y { get; set; }
    public bool OnCurve { get; set; }
}

public class CompositeGlyph
{
    public List<Component> Components { get; set; } = new();
    public ushort InstructionLength { get; set; }
    public byte[] Instructions { get; set; } = [];
}

public class Component
{
    public ComponentGlyphFlags Flags { get; set; }
    public ushort GlyphIndex { get; set; }
    public short? Argument1 { get; set; }
    public short? Argument2 { get; set; }
    public float? Scale { get; set; }
    public (float x, float y)? ScaleXY { get; set; }
    public float? Scale01 { get; set; }
    public float? Scale10 { get; set; }
}

