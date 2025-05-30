using FontConverter.SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FontConverter.SharedLibrary.Helpers;

public static class OrganizeGlyphsHelper
{
    public static async Task<SortedList<int, LVGLGlyph>> OrganizeGlyphsAsync(
        PredefinedData predefinedData,
        OpenTypeFont openTypeFont,
        LVGLFont lVGLFont,
        SortedList<int, LVGLGlyphBitmapData> glyphsRenderData,
        IProgress<(int glyphIndex, double percentage)>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var glyphs = new SortedList<int, LVGLGlyph>(openTypeFont.GlyfTable.Glyphs.Count);
        var totalGlyphs = openTypeFont.GlyfTable.Glyphs.Count;
        int chunkSize = Math.Max(1, totalGlyphs / 1000);
        if (chunkSize > 100) chunkSize = 100;
        var processedGlyphs = 0;
        var bitmapIndex = 0;

        var glyphNameIndex = openTypeFont.PostTable.GlyphNameIndex;
        var pascalStrings = openTypeFont.PostTable.PascalStrings;
        var standardMacGlyphNames = predefinedData.StandardMacintoshGlyphNames;
        var scale = lVGLFont.FontSettings.FontSize / (double)openTypeFont.HeadTable.UnitsPerEm;
        var glyphMetrics = openTypeFont.HmtxTable.GlyphMetrics;
        var glyphToUnicodeMap = openTypeFont.CmapTable.GlyphToUnicodeMap;

        for (int i = 0; i < totalGlyphs; i += chunkSize)
        {
            cancellationToken.ThrowIfCancellationRequested();
            int batchEnd = Math.Min(i + chunkSize, totalGlyphs);

            for (int j = i; j < batchEnd; j++)
            {
                var lvglGlyph = new LVGLGlyph
                {
                    Index = j,
                    Description = $"Glyph {j}",
                    Adjusments = lVGLFont.FontAdjusments
                };

                lvglGlyph.Name = glyphNameIndex != null && j < glyphNameIndex.Count
                    ? GetGlyphName(j, glyphNameIndex, pascalStrings, standardMacGlyphNames)
                    : $"Glyph_{j}";

                var renderData = glyphsRenderData[j];
                lvglGlyph.Bitmap = renderData.Bitmap;
                lvglGlyph.Descriptor = new LVGLGlyphDescriptor
                {
                    Width = renderData.Bounds.Width,
                    Height = renderData.Bounds.Height,
                    OffsetX = renderData.Bounds.Left,
                    OffsetY = -renderData.Bounds.Bottom,
                    AdvanceWidth = (int)Math.Ceiling(scale * glyphMetrics[j].AdvanceWidth),
                    BitmapIndex = bitmapIndex
                };
                bitmapIndex += lvglGlyph.Bitmap.Length;
                lvglGlyph.IsEmpty = lvglGlyph.Bitmap.Length == 0;


                glyphToUnicodeMap.TryGetValue((ushort)j, out var codePoints);

                FillGlyphFromCodePoints(lvglGlyph, codePoints, predefinedData.UnicodeBlockCollection);



                glyphs.Add(j, lvglGlyph);
                processedGlyphs++;
            }

            progress?.Report((processedGlyphs, (double)processedGlyphs / totalGlyphs * 100));
            var delay = Math.Max(1, chunkSize / 50);
            await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
        }

        progress?.Report((totalGlyphs, 100.0));
        return glyphs;
    }

    private static string GetGlyphName(
        int glyphIndex, 
        IList<ushort>? glyphNameIndex, 
        IList<string>? pascalStrings, 
        SortedList<int, string> standardMacGlyphNames)
    {
        if (glyphNameIndex == null || glyphIndex >= glyphNameIndex.Count)
            return $"Glyph_{glyphIndex}";

        ushort nameIndex = glyphNameIndex[glyphIndex];
        if (nameIndex > 257 && pascalStrings != null && nameIndex - 258 < pascalStrings.Count)
            return pascalStrings[nameIndex - 258];

        if (standardMacGlyphNames.TryGetValue(nameIndex, out string? glyphName))
            return glyphName;

        return $"Glyph_{glyphIndex}";
    }

    public static void FillGlyphFromCodePoints(
    LVGLGlyph glyph,
    List<uint>? codePoints,
    UnicodeBlockCollection blockCollection)
    {
        glyph.CodePoints.Clear();
        glyph.Blocks.Clear();

        var addedBlocks = new HashSet<(uint Start, uint End)>();
        if (codePoints is not null)
        {
            foreach (uint codePoint in codePoints)
            {
                if (!blockCollection.AllCharacters.TryGetValue(codePoint, out var unicodeChar))
                {
                    unicodeChar = new UnicodeCharacter(codePoint, $"U+{codePoint:X4}");
                }

                glyph.CodePoints[codePoint] = unicodeChar;

                foreach (var blockEntry in blockCollection.Blocks)
                {
                    var (start, end) = blockEntry.Key;
                    if (codePoint >= start && codePoint <= end)
                    {
                        if (addedBlocks.Add((start, end)))
                        {
                            glyph.Blocks[(start, end)] = blockEntry.Value;
                        }
                        break;
                    }
                }
            }
        }

        int count = glyph.CodePoints.Count;
        glyph.IsUnMapped = count == 0;
        glyph.IsSingleMapped = count == 1;
        glyph.IsMultiMapped = count > 1;
    }

}