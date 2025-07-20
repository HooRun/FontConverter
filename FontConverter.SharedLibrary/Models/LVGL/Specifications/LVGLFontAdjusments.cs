
using static FontConverter.SharedLibrary.Helpers.LVGLFontEnums;

namespace FontConverter.SharedLibrary.Models;

public class LVGLFontAdjusments
{
    public LVGLFontAdjusments()
    {
        AntiAlias = true;
        Dither = true;
        ColorFilter = true;
        Shader = true;
        Style = GLYPH_STYLE.STYLE_FILL;
        StrokeWidth = 0;
        Gamma = 50;
        Threshold = 0;
    }

    public LVGLFontAdjusments(bool antiAlias, bool dither, bool colorFilter, bool shader, GLYPH_STYLE style, int strokeWidth, int gamma, int threshold) : this()
    {
        AntiAlias = antiAlias;
        Dither = dither;
        ColorFilter = colorFilter;
        Shader = shader;
        Style = style;
        StrokeWidth = strokeWidth;
        Gamma = gamma;
        Threshold = threshold;
    }

    public bool AntiAlias { get; set; }
    public bool Dither { get; set; }
    public bool ColorFilter { get; set; }
    public bool Shader { get; set; }
    public GLYPH_STYLE Style { get; set; }
    public int StrokeWidth { get; set; }
    public int Gamma { get; set; }
    public int Threshold { get; set; }
}
