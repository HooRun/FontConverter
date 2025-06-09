using FontConverter.SharedLibrary.Models;

namespace FontConverter.Blazor.Models.GlyphsView;

public class GlyphItem : LVGLGlyph
{
    public GlyphItem()
    {
        IsSelected = false;
        IsHovered = false;
        Tooltip = string.Empty;
    }

    public GlyphItem(LVGLGlyph glyph) : this()
    {
        Index = glyph.Index;
        Name = glyph.Name;
        Description = glyph.Description;
        Bitmap = glyph.Bitmap;
        Descriptor = glyph.Descriptor;
        Adjusments = glyph.Adjusments;
        CodePoints = glyph.CodePoints;
        Blocks = glyph.Blocks;
        IsEmpty = glyph.IsEmpty;
        IsUnMapped = glyph.IsUnMapped;
        IsSingleMapped = glyph.IsSingleMapped;
        IsMultiMapped = glyph.IsMultiMapped;
    }

    public bool IsSelected { get; set; }
    public bool IsHovered { get; set; }
    public string Tooltip { get; set; }
}
