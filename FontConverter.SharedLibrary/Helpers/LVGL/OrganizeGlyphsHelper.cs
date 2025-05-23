using FontConverter.SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FontConverter.SharedLibrary.Helpers;

public class OrganizeGlyphsHelper
{
    public static async Task<SortedList<int, LVGLGlyph>> OrganizeGlyphs(
        PredefinedData predefinedData,
        OpenTypeFont openType,
        SortedList<int, LVGLGlyphBitmapData> glyphsRenderData,
        [AllowNull] IProgress<(int glyphIndex, double percentage)> progress = null,
        CancellationToken cancellationToken = default)
    {
        SortedList<int, LVGLGlyph> glyphs = new();

        return glyphs;
    }
}
