namespace FontConverter.SharedLibrary.Models;

public class LVGLFont
{
    public LVGLFont()
    {
        FontSettings = new();
        FontAdjusments = new();
        FontInformations = new();
        
        Glyphs = new();
    }

    public LVGLFontSettings FontSettings { get; set; }
    public LVGLFontAdjusments FontAdjusments { get; set; }
    public LVGLFontInformations FontInformations { get; set; }

    public SortedList<int, LVGLGlyph> Glyphs { get; set; }



}

