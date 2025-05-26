using FontConverter.SharedLibrary.Models;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace FontConverter.SharedLibrary.Helpers;

public static class FinalizingFontHelper
{
    public static async Task FinalizingFontAsync(
        SKTypeface typeface,
        SKFont font,
        OpenTypeFont openTypeFont,
        LVGLFont lVGLFont,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default)
    {

        var scale = lVGLFont.FontSettings.FontSize / (double)openTypeFont.HeadTable.UnitsPerEm;

        lVGLFont.FontInformations = new();
        lVGLFont.FontInformations.FontName = typeface.FamilyName;
        lVGLFont.FontInformations.LineHeight = ((int)(Math.Ceiling(scale * openTypeFont.OS2Table.UsWinAscent) + Math.Ceiling(scale * openTypeFont.OS2Table.UsWinDescent))).ToString();
        lVGLFont.FontInformations.BaseLine = Math.Ceiling(scale * openTypeFont.OS2Table.UsWinDescent).ToString();
        lVGLFont.FontInformations.CharWidthMax = Math.Ceiling(font.Metrics.MaxCharacterWidth).ToString();
        lVGLFont.FontInformations.AdvanceWidthMax = Math.Ceiling(scale * openTypeFont.HheaTable.AdvanceWidthMax).ToString();
        lVGLFont.FontInformations.UnderlinePosition = Math.Ceiling(scale * openTypeFont.PostTable.UnderlinePosition).ToString();
        lVGLFont.FontInformations.UnderlineThickness = Math.Ceiling(scale * openTypeFont.PostTable.UnderlineThickness).ToString();
        lVGLFont.FontInformations.Ascent = Math.Ceiling(scale * openTypeFont.OS2Table.UsWinAscent).ToString();
        lVGLFont.FontInformations.Descent = Math.Ceiling(scale * openTypeFont.OS2Table.UsWinDescent).ToString();
        lVGLFont.FontInformations.XMin = Math.Ceiling(scale * openTypeFont.HeadTable.XMin).ToString();
        lVGLFont.FontInformations.YMin = Math.Ceiling(scale * openTypeFont.HeadTable.YMin).ToString();
        lVGLFont.FontInformations.XMax = Math.Ceiling(scale * openTypeFont.HeadTable.XMax).ToString();
        lVGLFont.FontInformations.YMax = Math.Ceiling(scale * openTypeFont.HeadTable.YMax).ToString();
        progress?.Report(50.0);
        await Task.Delay(500, cancellationToken).ConfigureAwait(false);

        lVGLFont.FontContents = new();
        lVGLFont.FontContents.Contents[lVGLFont.FontContents.GlyphsHeader].Count= lVGLFont.Glyphs.Count;
        lVGLFont.FontContents.Contents[lVGLFont.FontContents.UnicodesHeader].Count = openTypeFont.CmapTable.UnicodeToGlyphMap.Count;
        foreach (var glyph in lVGLFont.Glyphs.Values)
        {
            if (glyph.IsEmpty) lVGLFont.FontContents
                    .Contents[lVGLFont.FontContents.GlyphsHeader]
                    .Contents[lVGLFont.FontContents.EmptyGlyphsHeader]
                    .Count++;
            if (glyph.IsUnMapped) lVGLFont.FontContents
                    .Contents[lVGLFont.FontContents.GlyphsHeader]
                    .Contents[lVGLFont.FontContents.UnMappedGlyphsHeader]
                    .Count++;
            if (glyph.IsSingleMapped) lVGLFont.FontContents
                    .Contents[lVGLFont.FontContents.GlyphsHeader]
                    .Contents[lVGLFont.FontContents.SingleMappedGlyphsHeader]
                    .Count++;
            if (glyph.IsMultiMapped) lVGLFont.FontContents
                    .Contents[lVGLFont.FontContents.GlyphsHeader]
                    .Contents[lVGLFont.FontContents.MultiMappedGlyphsHeader]
                    .Count++;

            foreach (var range in glyph.Blocks.Values)
            {
                if (!lVGLFont.FontContents
                    .Contents[lVGLFont.FontContents.UnicodesHeader]
                    .Contents.ContainsKey(range.Name))
                {
                    lVGLFont.FontContents
                        .Contents[lVGLFont.FontContents.UnicodesHeader]
                        .Contents
                        .TryAdd(range.Name, new LVGLFontContent(range.Name, lVGLFont.FontContents.UnicodeRangeIcon, 1, false, new SortedList<string, LVGLFontContent>()));
                }
                else
                {
                    lVGLFont.FontContents
                        .Contents[lVGLFont.FontContents.UnicodesHeader]
                        .Contents[range.Name].Count++;
                }
            }
        }
        progress?.Report(100.0);
        await Task.Delay(500, cancellationToken).ConfigureAwait(false);
    }
}
