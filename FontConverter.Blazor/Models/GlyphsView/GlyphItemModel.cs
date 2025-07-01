using FontConverter.Blazor.ViewModels;
using FontConverter.SharedLibrary.Models;

namespace FontConverter.Blazor.Models.GlyphsView;

public class GlyphItemModel : LVGLGlyph
{
    public GlyphItemModel()
    {
        Index = -1;
        Name = string.Empty;
        Description = string.Empty;
        Bitmap = Array.Empty<byte>();
        Descriptor = new();
        Adjusments = new();
        CodePoints = new();
        Blocks = new();

        IsEmpty = false;
        IsUnMapped = false;
        IsSingleMapped = false;
        IsMultiMapped = false;

        IsSelected = false;
        IsHovered = false;
        LastSelected = false;
        Tooltip = string.Empty;
    }

    public GlyphItemModel(LVGLGlyph glyph) : this()
    {
        Index = glyph.Index;
        Name = glyph.Name;
        Description = glyph.Description;
        Bitmap = glyph.Bitmap;
        Descriptor = glyph.Descriptor;
        Adjusments = glyph.Adjusments;
        CodePoints = glyph.CodePoints;
        Blocks = glyph.Blocks;
        LeftKernings = glyph.LeftKernings;
        RightKernings = glyph.RightKernings;
        SVG = glyph.SVG;
        IsEmpty = glyph.IsEmpty;
        IsUnMapped = glyph.IsUnMapped;
        IsSingleMapped = glyph.IsSingleMapped;
        IsMultiMapped = glyph.IsMultiMapped;
    }


    public bool IsSelected { get; set; }
    public bool IsHovered { get; set; }
    public bool LastSelected { get; set; }
    public string Tooltip { get; set; }
}
