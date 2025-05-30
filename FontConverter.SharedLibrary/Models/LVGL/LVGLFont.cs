namespace FontConverter.SharedLibrary.Models;

public class LVGLFont
{
    public LVGLFont()
    {
        FontSettings = new();
        FontAdjusments = new();
        FontInformations = new();
        FontContents = new();
        GlyphViewItemProperties = new();
        Glyphs = new();
        Blocks = new();
    }

    public LVGLFontSettings FontSettings { get; set; }
    public LVGLFontAdjusments FontAdjusments { get; set; }
    public LVGLFontInformations FontInformations { get; set; }
    public LVGLFontContents FontContents { get; set; }
    public LVGLGlyphViewItemProperties GlyphViewItemProperties { get; set; }

    public SortedList<int, LVGLGlyph> Glyphs { get; set; }

    public SortedList<(int Start, int End), UnicodeBlock> Blocks { get; set; }

}

