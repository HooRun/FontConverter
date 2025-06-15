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
        OpenTypeFont openTypeFont,
        LVGLFont lvglFont,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default)
    {

        UpdateFontInformation(openTypeFont, lvglFont);
        progress?.Report(10.0);
        await Task.Delay(500, cancellationToken).ConfigureAwait(false);

        UpdateGlyphViewItemProperties(openTypeFont, lvglFont);
        progress?.Report(40.0);
        await Task.Delay(500, cancellationToken).ConfigureAwait(false);

        UpdateFontContent(openTypeFont, lvglFont);
        progress?.Report(90.0);
        await Task.Delay(500, cancellationToken).ConfigureAwait(false);
    }

    private static void UpdateFontInformation(
        OpenTypeFont openTypeFont, 
        LVGLFont lvglFont)
    {
        var scale = lvglFont.FontSettings.FontSize / (double)openTypeFont.HeadTable.UnitsPerEm;

        lvglFont.FontInformations = new();
        lvglFont.FontInformations.FontName = openTypeFont.SKTypeface!.FamilyName;
        lvglFont.FontInformations.LineHeight = ((int)(Math.Ceiling(scale * openTypeFont.OS2Table.UsWinAscent) + Math.Ceiling(scale * openTypeFont.OS2Table.UsWinDescent))).ToString();
        lvglFont.FontInformations.BaseLine = ((int)Math.Ceiling(scale * openTypeFont.OS2Table.UsWinDescent)).ToString();
        lvglFont.FontInformations.CharWidthMax = ((int)Math.Ceiling(openTypeFont.SKFont!.Metrics.MaxCharacterWidth)).ToString();
        lvglFont.FontInformations.AdvanceWidthMax = ((int)Math.Ceiling(scale * openTypeFont.HheaTable.AdvanceWidthMax)).ToString();
        lvglFont.FontInformations.UnderlinePosition = ((int)Math.Ceiling(scale * openTypeFont.PostTable.UnderlinePosition)).ToString();
        lvglFont.FontInformations.UnderlineThickness = ((int)Math.Ceiling(scale * openTypeFont.PostTable.UnderlineThickness)).ToString();
        lvglFont.FontInformations.Ascent = ((int)Math.Ceiling(scale * openTypeFont.OS2Table.UsWinAscent)).ToString();
        lvglFont.FontInformations.Descent = ((int)Math.Ceiling(scale * openTypeFont.OS2Table.UsWinDescent)).ToString();
        lvglFont.FontInformations.XMin = ((int)Math.Ceiling(scale * openTypeFont.HeadTable.XMin)).ToString();
        lvglFont.FontInformations.YMin = ((int)Math.Ceiling(scale * openTypeFont.HeadTable.YMin)).ToString();
        lvglFont.FontInformations.XMax = ((int)Math.Ceiling(scale * openTypeFont.HeadTable.XMax)).ToString();
        lvglFont.FontInformations.YMax = ((int)Math.Ceiling(scale * openTypeFont.HeadTable.YMax)).ToString();
    }

    public static void UpdateGlyphViewItemProperties(
        OpenTypeFont openTypeFont,
        LVGLFont lvglFont)
    {
        var scale = lvglFont.FontSettings.FontSize / (double)openTypeFont.HeadTable.UnitsPerEm;

        lvglFont.GlyphViewItemProperties.XMin = (int)Math.Ceiling(scale * openTypeFont.HeadTable.XMin);
        lvglFont.GlyphViewItemProperties.BaseLine = (int)Math.Ceiling(scale * openTypeFont.OS2Table.UsWinDescent);
        lvglFont.GlyphViewItemProperties.ItemWidth = (int)Math.Max(Math.Ceiling(openTypeFont.SKFont!.Metrics.MaxCharacterWidth), Math.Ceiling(scale * openTypeFont.HheaTable.AdvanceWidthMax));
        lvglFont.GlyphViewItemProperties.ItemHeight = (int)(Math.Ceiling(scale * openTypeFont.OS2Table.UsWinAscent) + Math.Ceiling(scale * openTypeFont.OS2Table.UsWinDescent));
        lvglFont.GlyphViewItemProperties.Zoom = 1;
    }

    private static void UpdateFontContent(
        OpenTypeFont openTypeFont,
        LVGLFont lvglFont)
    {
        lvglFont.FontContents = new();
        lvglFont.FontContents.Contents[lvglFont.FontContents.GlyphsHeader].Count = lvglFont.Glyphs.Count;
        lvglFont.FontContents.Contents[lvglFont.FontContents.UnicodesHeader].Count = openTypeFont.CmapTable.UnicodeToGlyphMap.Count;
        foreach (var glyph in lvglFont.Glyphs.Values)
        {
            lvglFont.FontContents
                    .Contents[lvglFont.FontContents.GlyphsHeader]
                    .Items.Add(glyph.Index);
            if (glyph.IsEmpty)
            {
                lvglFont.FontContents
                    .Contents[lvglFont.FontContents.GlyphsHeader]
                    .Contents[lvglFont.FontContents.EmptyGlyphsHeader]
                    .Count++;
                lvglFont.FontContents
                    .Contents[lvglFont.FontContents.GlyphsHeader]
                    .Contents[lvglFont.FontContents.EmptyGlyphsHeader]
                    .Items.Add(glyph.Index);
            }
            if (glyph.IsUnMapped)
            {
                lvglFont.FontContents
                    .Contents[lvglFont.FontContents.GlyphsHeader]
                    .Contents[lvglFont.FontContents.UnMappedGlyphsHeader]
                    .Count++;
                lvglFont.FontContents
                    .Contents[lvglFont.FontContents.GlyphsHeader]
                    .Contents[lvglFont.FontContents.UnMappedGlyphsHeader]
                    .Items.Add(glyph.Index);
            }
            if (glyph.IsSingleMapped)
            {
                lvglFont.FontContents
                    .Contents[lvglFont.FontContents.GlyphsHeader]
                    .Contents[lvglFont.FontContents.SingleMappedGlyphsHeader]
                    .Count++;
                lvglFont.FontContents
                    .Contents[lvglFont.FontContents.GlyphsHeader]
                    .Contents[lvglFont.FontContents.SingleMappedGlyphsHeader]
                    .Items.Add(glyph.Index);
            }
            if (glyph.IsMultiMapped)
            {
                lvglFont.FontContents
                    .Contents[lvglFont.FontContents.GlyphsHeader]
                    .Contents[lvglFont.FontContents.MultiMappedGlyphsHeader]
                    .Count++;
                lvglFont.FontContents
                    .Contents[lvglFont.FontContents.GlyphsHeader]
                    .Contents[lvglFont.FontContents.MultiMappedGlyphsHeader]
                    .Items.Add(glyph.Index);
            }

            foreach (var range in glyph.Blocks.Values)
            {
                if (!lvglFont.FontContents
                    .Contents[lvglFont.FontContents.UnicodesHeader]
                    .Contents.ContainsKey(range.Start.ToString()))
                {
                    string subTitle = $"Range: 0x{range.Start:X04} - 0x{range.End:X04}";
                    lvglFont.FontContents
                        .Contents[lvglFont.FontContents.UnicodesHeader]
                        .Contents
                        .TryAdd(range.Start.ToString(), new LVGLFontContent(range.Name, subTitle, lvglFont.FontContents.UnicodeRangeIcon, 1, false, null, new SortedList<string, LVGLFontContent>()));
                    lvglFont.FontContents
                        .Contents[lvglFont.FontContents.UnicodesHeader]
                        .Contents[range.Start.ToString()]
                        .Items.Add(glyph.Index);
                }
                else
                {
                    lvglFont.FontContents
                        .Contents[lvglFont.FontContents.UnicodesHeader]
                        .Contents[range.Start.ToString()]
                        .Count++;
                    lvglFont.FontContents
                        .Contents[lvglFont.FontContents.UnicodesHeader]
                        .Contents[range.Start.ToString()]
                        .Items.Add(glyph.Index);
                }
            }
        }
    }
}
