using FontConverter.SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FontConverter.SharedLibrary.Helpers.LVGLFontEnums;

namespace FontConverter.SharedLibrary.Helpers;

public static class ExportCmapHelper
{
    public static List<LVGLCMapRange> GenerateCMapRangesByUnicodeBlocks(
        IEnumerable<LVGLGlyph> glyphs,
        SortedDictionary<uint, UnicodeBlock> blocks)
    {
        var cmapRanges = new List<LVGLCMapRange>();

        foreach (var block in blocks.Values)
        {
            var characters = block.Characters.Values
                .Where(x => x.GlyphID != null && x.GlyphID >= 0)
                .ToList();

            if (characters == null || characters.Count == 0)
                continue;

            var unicodeCodepoints = characters
                .Select(x => x.CodePoint)
                .ToList();

            var glyphIDs = characters
                .Select(x => (int)x.GlyphID!)
                .ToList();

            var cmap = new LVGLCMapRange
            {
                BlockName = block.Name.Replace(" ", "_").ToLowerInvariant(),
                Block = block,
                RangeStart = (int)unicodeCodepoints.First(),
                RangeLength = (int)(unicodeCodepoints.Last() - unicodeCodepoints.First() +  1),
                GlyphIDStart = glyphIDs.Min()
            };

            var rangeStart = (uint)cmap.RangeStart;
            var glyphIDStart = cmap.GlyphIDStart;

            var unicodeOffsets = unicodeCodepoints.Select(cp => (ushort)(cp - rangeStart)).ToList();
            var glyphIdOffsets = glyphIDs.Select(gid => (ushort)(gid - glyphIDStart)).ToList();

            bool unicodeContiguous = IsContiguous(unicodeCodepoints);
            bool glyphContiguous = IsContiguous(glyphIDs);
            bool glyphsFitInByte = glyphIDs.All(gid => gid - glyphIDStart <= 255);
            bool isDirectMap = unicodeOffsets.Count == glyphIdOffsets.Count &&
                               !unicodeOffsets.Where((t, i) => t != glyphIdOffsets[i]).Any();
            bool hasZeroInMiddle = glyphIdOffsets.Skip(1).Any(offset => offset == 0);

            if (glyphsFitInByte && unicodeContiguous && glyphContiguous && isDirectMap)
            {
                cmap.Type = LVGL_CMAP_TYPE.LV_FONT_FMT_TXT_CMAP_FORMAT0_TINY;
                cmap.ListLength = 0;
            }
            else if (glyphsFitInByte && unicodeContiguous && !hasZeroInMiddle)
            {
                cmap.Type = LVGL_CMAP_TYPE.LV_FONT_FMT_TXT_CMAP_FORMAT0_FULL;
                cmap.GlyphIDOffsetList = glyphIdOffsets;
                cmap.GlyphIDOffsetListName = $"glyph_id_ofs_list_{rangeStart:X}";
                cmap.ListLength = glyphIdOffsets.Count;
            }
            else if (glyphContiguous)
            {
                cmap.Type = LVGL_CMAP_TYPE.LV_FONT_FMT_TXT_CMAP_SPARSE_TINY;
                cmap.UnicodeList = unicodeOffsets;
                cmap.UnicodeListName = $"unicode_list_{rangeStart:X}";
                cmap.ListLength = unicodeOffsets.Count;
            }
            else
            {
                cmap.Type = LVGL_CMAP_TYPE.LV_FONT_FMT_TXT_CMAP_SPARSE_FULL;
                cmap.UnicodeList = unicodeOffsets;
                cmap.GlyphIDOffsetList = glyphIdOffsets;
                cmap.UnicodeListName = $"unicode_list_{rangeStart:X}";
                cmap.GlyphIDOffsetListName = $"glyph_id_ofs_list_{rangeStart:X}";
                cmap.ListLength = unicodeOffsets.Count;
            }


            cmapRanges.Add(cmap);
        }

        return cmapRanges;
    }

    private static bool IsContiguous(List<uint> list)
    {
        for (int i = 1; i < list.Count; i++)
        {
            if (list[i] != list[i - 1] + 1)
                return false;
        }
        return true;
    }

    private static bool IsContiguous(List<int> list)
    {
        for (int i = 1; i < list.Count; i++)
        {
            if (list[i] != list[i - 1] + 1)
                return false;
        }
        return true;
    }
}


