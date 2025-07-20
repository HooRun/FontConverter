using static LVGLFontConverter.Library.Helpers.LVGLFontEnums;

namespace LVGLFontConverter.Models;

public class GlyphStyle
{
    public GlyphStyle()
    {
        
    }

    public GLYPH_STYLE Style { get; set; }
    public string Description { get; set; } = string.Empty;
}
