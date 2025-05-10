using static LVGLFontConverter.Library.Helpers.LVGLFontEnums;

namespace LVGLFontConverter.Library.Models;

public class LVGLFontAdjusment
{
    public LVGLFontAdjusment()
    {
        AntiAlias = true;
        Dither = true;
        ColorFilter = true;
        Shader = true;
        Style = GLYPH_STYLE.STYLE_FILL;
        Gamma = 50;
        Threshold = 0;
    }

    public bool AntiAlias { get; set; }
    public bool Dither { get; set; }
    public bool ColorFilter { get; set; }
    public bool Shader { get; set; }
    public GLYPH_STYLE Style { get; set; }
    public int Gamma { get; set; }
    public int Threshold { get; set; }
}
