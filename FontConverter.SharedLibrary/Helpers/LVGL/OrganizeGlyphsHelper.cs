using FontConverter.SharedLibrary.Models;
using System;
using System.Collections;
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
        CancellationToken cancellationToken = default,
        bool justRender = false)
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

        List<KernPair> kernPairs = [];
        if (!justRender)
        {
            kernPairs = openTypeFont.KernTable.AllPairs
            .Select(k => new KernPair
            {
                Left = k.Left,
                Right = k.Right,
                Value = k.Value >= 0 ? (short)Math.Ceiling(scale * k.Value) : (short)Math.Floor(scale * k.Value)
            })
            .ToList();
        }

        for (int i = 0; i < totalGlyphs; i += chunkSize)
        {
            cancellationToken.ThrowIfCancellationRequested();
            int batchEnd = Math.Min(i + chunkSize, totalGlyphs);

            for (int j = i; j < batchEnd; j++)
            {
                var lvglGlyph = new LVGLGlyph
                {
                    Index = processedGlyphs,
                    Description = $"Glyph {processedGlyphs}",
                };

                if (!justRender)
                {
                    lvglGlyph.Name = GetGlyphName(processedGlyphs, glyphNameIndex, pascalStrings, standardMacGlyphNames);
                }

                var renderData = glyphsRenderData[processedGlyphs];
                lvglGlyph.Bitmap = renderData.Bitmap;
                lvglGlyph.SVG = renderData.SVG;
                lvglGlyph.Descriptor = new LVGLGlyphDescriptor(
                    processedGlyphs,
                    renderData.Bounds.Width,
                    renderData.Bounds.Height,
                    renderData.Bounds.Left,
                    -renderData.Bounds.Bottom,
                    (int)Math.Ceiling(scale * glyphMetrics[j].AdvanceWidth));
                
                lvglGlyph.Adjusments = new LVGLFontAdjusments(
                    lVGLFont.FontAdjusments.AntiAlias,
                    lVGLFont.FontAdjusments.Dither,
                    lVGLFont.FontAdjusments.ColorFilter,
                    lVGLFont.FontAdjusments.Shader,
                    lVGLFont.FontAdjusments.Style,
                    lVGLFont.FontAdjusments.StrokeWidth,
                    lVGLFont.FontAdjusments.Gamma,
                    lVGLFont.FontAdjusments.Threshold
                    );
                bitmapIndex += lvglGlyph.Bitmap.Length;
                lvglGlyph.IsEmpty = lvglGlyph.Bitmap.Length == 0;

                if (!justRender)
                {
                    glyphToUnicodeMap.TryGetValue((ushort)processedGlyphs, out var codePoints);
                    FillGlyphFromCodePoints(lvglGlyph, codePoints, predefinedData.Blocks);
                    lvglGlyph.LeftKernings = kernPairs.Where(p => p.Left == processedGlyphs).OrderBy(p => p.Left).ToList();
                    lvglGlyph.RightKernings = kernPairs.Where(p => p.Right == processedGlyphs).OrderBy(p => p.Right).ToList();
                }



                glyphs.Add(processedGlyphs, lvglGlyph);
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
        List<string> sm = [];
        sm.AddRange(standardMacGlyphNames.Values);
        if (nameIndex < sm.Count)
            return sm[nameIndex];

        if (standardMacGlyphNames.TryGetValue(nameIndex, out string? glyphName))
            return glyphName;

        return $"Glyph_{glyphIndex}";
    }

    public static void FillGlyphFromCodePoints(
    LVGLGlyph glyph,
    List<uint>? codePoints,
    SortedDictionary<uint, UnicodeBlock> blockCollection)
    {
        glyph.CodePoints.Clear();
        glyph.Blocks.Clear();
        if (codePoints is not null)
        {
            foreach (uint codePoint in codePoints)
            {
                foreach (var blockEntry in blockCollection.Values)
                {
                    if (blockEntry.Start > codePoint &&  codePoint > blockEntry.End)
                        continue;

                    if (blockEntry.Start <=codePoint && codePoint <= blockEntry.End)
                    {
                        if (!glyph.Blocks.ContainsKey(blockEntry.Start))
                        {
                            glyph.Blocks.TryAdd(blockEntry.Start, blockEntry);
                        }
                        if (blockEntry.Characters.ContainsKey(codePoint))
                        {
                            blockEntry.Characters[codePoint].GlyphID = glyph.Index;
                            if (!glyph.CodePoints.ContainsKey(codePoint))
                            {
                                glyph.CodePoints.TryAdd(codePoint, blockEntry.Characters[codePoint]);
                            }
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