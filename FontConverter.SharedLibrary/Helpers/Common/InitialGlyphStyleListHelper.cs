using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FontConverter.SharedLibrary.Helpers;

public static class InitialGlyphStyleListHelper
{
    public static async Task<SortedList<LVGLFontEnums.GLYPH_STYLE, string>> InitialGlyphStyleList(CancellationToken cancellationToken = default)
    {
        SortedList<LVGLFontEnums.GLYPH_STYLE, string> glyphStylelList = new();
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                glyphStylelList.Add(LVGLFontEnums.GLYPH_STYLE.STYLE_FILL, "Fill");
                glyphStylelList.Add(LVGLFontEnums.GLYPH_STYLE.STYLE_STROKE, "Stroke");
                glyphStylelList.Add(LVGLFontEnums.GLYPH_STYLE.STYLE_FILL_STROKE, "Fill and Stroke");
            }, cancellationToken);

        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            throw;
        }
        return glyphStylelList;
    }
}
