using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FontConverter.SharedLibrary.Helpers;

public static class InitialGlyphStyleListHelper
{
    private static readonly SortedList<LVGLFontEnums.GLYPH_STYLE, string> _GlyphStyleList = new()
    {
        { LVGLFontEnums.GLYPH_STYLE.STYLE_FILL, "Fill" },
        { LVGLFontEnums.GLYPH_STYLE.STYLE_STROKE, "Stroke" },
        { LVGLFontEnums.GLYPH_STYLE.STYLE_FILL_STROKE, "Fill and Stroke" }
    };

    public static SortedList<LVGLFontEnums.GLYPH_STYLE, string> InitialGlyphStyleList()
    {
        return _GlyphStyleList;
    }
}
