using FontConverter.SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FontConverter.SharedLibrary.Helpers.LVGLFontEnums;

namespace FontConverter.SharedLibrary.Helpers;

public static class ExportToCHelper
{
    private const string _Tab1 = "    ";
    private const string _Tab2 = "        ";
    private const string _Tab3 = "            ";
    private const string _Tab4 = "                ";

    private const string _BitmapArrayName = "glyph_bitmap";
    private const string _DescriptorArrayName = "glyph_dsc";
    private const string _CMapsArrayName = "cmaps";
    private const string _KerningLeftMapName = "kern_left_class_mapping";
    private const string _KerningRightMapName = "kern_right_class_mapping";
    private const string _KerningValuesName = "kern_class_values";
    private const string _KernClassesName = "kern_classes";
    private const string _FontDescriptorName = "font_dsc";

    public static string ExportToC(LVGLFont lvglFont, IList<LVGLGlyph> glyphsToExport)
    {
        if (lvglFont == null || glyphsToExport == null || glyphsToExport.Count <= 0)
            return string.Empty;

        List<UnicodeCharacter> codePoints = GetGlyphsCodePoints(glyphsToExport);
        
        List<LVGLCMapRange> cmaps = GenerateCMapRanges(codePoints);
        bool haveCMaps = cmaps.Count > 0;
        int cMapsCount = cmaps.Count;

        int kerningScale = 0;
        int kernClassesCount = 0;
        bool haveKernings = false;
        LVGLKerningClassResult kernResult = new();
        List<KernPair> pairs = CollectUniqueKernPairs(glyphsToExport.ToList());
        if (pairs.Count > 0)
        {
            kerningScale = ScaleKernPairsInPlace(pairs);
            kernResult = GenerateKerningClassTables(glyphsToExport.ToList(), pairs);
            haveKernings = kernResult.ClassValues.Length > 0;
            kernClassesCount = 1;
        }
        
        
        StringBuilder cFile = new();

        cFile.Append(HeaderSection(lvglFont));
        cFile.Append(BitmapSection(glyphsToExport));
        cFile.Append(DescriptorSection(glyphsToExport));
        if (haveCMaps)
        {
            cFile.Append(CMapSection(cmaps));
        }
        if (haveCMaps && haveKernings)
        {
            cFile.Append(KerningSection(kernResult));
        }
        cFile.Append(FooterSection(lvglFont, haveKernings, kerningScale, cMapsCount, kernClassesCount));
        return cFile.ToString();
    }

    private static StringBuilder HeaderSection(LVGLFont lvglFont)
    {
        StringBuilder headerSection = new();
        int gammaValue = Math.Clamp((int)lvglFont.FontAdjusments.Gamma, 0, 100);
        float gamma;
        if (gammaValue <= 50)
        {
            gamma = gammaValue / 50.0f;
        }
        else
        {
            gamma = 1.0f + ((gammaValue - 50) * 9.0f / 50.0f);
        }


        headerSection.AppendLine("/*******************************************************************************");
        headerSection.AppendLine("* Font Converter For LVGL");
        headerSection.AppendLine("* https://hoorun.github.io/FontConverter/");
        headerSection.AppendLine("*");
        headerSection.AppendFormat("* OpenType Font Name: {0}", lvglFont.FontInformations.FontName).AppendLine();
        headerSection.AppendFormat("* Font Name: {0}",lvglFont.FontSettings.FontName).AppendLine();
        headerSection.AppendFormat("* Font BPP: {0}", lvglFont.FontSettings.FontBitPerPixel).AppendLine();
        headerSection.AppendFormat("* Font Size: {0}", lvglFont.FontSettings.FontSize).AppendLine();
        headerSection.AppendFormat("* Font Adjusments: AntiAlias={0}, Dither={1}, Style={2}, StrokeWidth={3}, Gamma={4}, Threshold={5}", 
            lvglFont.FontAdjusments.AntiAlias.ToString(),
            lvglFont.FontAdjusments.Dither.ToString(),
            lvglFont.FontAdjusments.Style.ToString(),
            lvglFont.FontAdjusments.StrokeWidth,
            gamma,
            lvglFont.FontAdjusments.Threshold).AppendLine();
        headerSection.AppendLine("******************************************************************************/");
        headerSection.AppendLine();
        headerSection.AppendLine("#ifdef LV_LVGL_H_INCLUDE_SIMPLE");
        headerSection.AppendLine($"{_Tab1}#include \"lvgl.h\"");
        headerSection.AppendLine("#else");
        headerSection.AppendLine($"{_Tab1}#include \"lvgl/lvgl.h\"");
        headerSection.AppendLine("#endif");
        headerSection.AppendLine();
        headerSection.AppendLine($"#ifndef CUSTOM_FONT_{lvglFont.FontSettings.FontName.ToUpper()}");
        headerSection.AppendLine($"{_Tab1}#define CUSTOM_FONT_{lvglFont.FontSettings.FontName.ToUpper()} 1");
        headerSection.AppendLine("#endif");
        headerSection.AppendLine();
        headerSection.AppendLine($"#if CUSTOM_FONT_{lvglFont.FontSettings.FontName.ToUpper()}");
        headerSection.AppendLine();
        return headerSection;
    }

    private static StringBuilder BitmapSection(IList<LVGLGlyph> glyphsToExport)
    {
        StringBuilder bitmapSection = new();
        bitmapSection.AppendLine("/*-----------------");
        bitmapSection.AppendLine("*    BITMAPS");
        bitmapSection.AppendLine("*----------------*/");
        bitmapSection.AppendLine();
        bitmapSection.AppendLine("/* Store the image of the glyphs */");
        bitmapSection.AppendLine($"/* Count of glyphs: {glyphsToExport.Count} */");
        bitmapSection.AppendLine($"static LV_ATTRIBUTE_LARGE_CONST const uint8_t {_BitmapArrayName}[] = {{");
        int glyphIndex = 0;
        foreach (var glyph in glyphsToExport)
        {
            bitmapSection.AppendLine($"{_Tab1}/* Index: {glyphIndex}, Name: {glyph.Name}, Description: {glyph.Description}, Unicodes: {glyph.UnicodesStrings}, Lenght: {glyph.Bitmap.Length} */");
            int columnCount = 0;
            bitmapSection.Append(_Tab1);
            for (int i = 0; i < glyph.Bitmap.Length; i++)
            {
                bitmapSection.AppendFormat("0x{0:X2}", glyph.Bitmap[i]);

                bool isLastByte = (glyphIndex == glyphsToExport.Count - 1) && (i == glyph.Bitmap.Length - 1);
                if (!isLastByte)
                    bitmapSection.Append(", ");

                columnCount++;

                if (columnCount == 16)
                {
                    bitmapSection.AppendLine();
                    bitmapSection.Append(_Tab1);
                    columnCount = 0;
                }
            }
            if (columnCount > 0)
            {
                bitmapSection.AppendLine();
            }
            bitmapSection.AppendLine();
            glyphIndex++;
        }
        bitmapSection.AppendLine("};");
        bitmapSection.AppendLine();
        return bitmapSection;
    }

    private static StringBuilder DescriptorSection(IList<LVGLGlyph> glyphsToExport)
    {
        StringBuilder descriptorSection = new();
        descriptorSection.AppendLine("/*---------------------");
        descriptorSection.AppendLine("*  GLYPH DESCRIPTION");
        descriptorSection.AppendLine("*--------------------*/");
        descriptorSection.AppendLine();
        descriptorSection.AppendLine($"static const lv_font_fmt_txt_glyph_dsc_t {_DescriptorArrayName}[] = {{");
        int glyphIndex = 0;
        int bitmapIndex = 0;
        foreach (var glyph in glyphsToExport)
        {
            descriptorSection.AppendFormat("{0}{{.bitmap_index = {1}, .adv_w = {2}, .box_w = {3}, .box_h = {4}, .ofs_x = {5}, .ofs_y = {6}}} /* Index = {7} */",
                _Tab1,
                bitmapIndex,
                glyph.Descriptor.AdvanceWidth * 16,
                glyph.Descriptor.Width,
                glyph.Descriptor.Height,
                glyph.Descriptor.OffsetX,
                glyph.Descriptor.OffsetY,
                glyphIndex);
            if (glyphIndex != (glyphsToExport.Count - 1))
                descriptorSection.Append(",");
            descriptorSection.AppendLine();
            bitmapIndex += glyph.Bitmap.Length;
            glyphIndex++;
        }
        descriptorSection.AppendLine("};");
        descriptorSection.AppendLine();
        return descriptorSection;
    }

    private static StringBuilder CMapSection(List<LVGLCMapRange> cmaps)
    {
        StringBuilder cMapSection = new();
        cMapSection.AppendLine("/*---------------------");
        cMapSection.AppendLine("*  CHARACTER MAPPING");
        cMapSection.AppendLine("*--------------------*/");
        cMapSection.AppendLine();
        
        foreach (var cmap in cmaps)
        {
            // --- Unicode List ---
            if (cmap.UnicodeList.Count > 0)
            {
                var listType = "uint16_t"; // LVGL uses uint16_t even for sparse tiny now

                cMapSection.AppendLine($"static const {listType} {cmap.UnicodeListName}[] = {{");

                for (int i = 0; i < cmap.UnicodeList.Count; i += 16)
                {
                    var chunk = cmap.UnicodeList
                        .Skip(i)
                        .Take(16)
                        .Select(u => $"0x{u:X}")
                        .ToList();

                    cMapSection.Append(_Tab1);
                    cMapSection.AppendLine(string.Join(", ", chunk) + (i + 16 >= cmap.UnicodeList.Count ? "" : ","));
                }

                cMapSection.AppendLine("};");
                cMapSection.AppendLine();
            }

            // --- Glyph ID Offset List ---
            if (cmap.GlyphIDOffsetList.Count > 0)
            {
                var listType = cmap.Type == LVGL_CMAP_TYPE.LV_FONT_FMT_TXT_CMAP_FORMAT0_FULL
                    ? "uint8_t"
                    : "uint16_t";

                cMapSection.AppendLine($"static const {listType} {cmap.GlyphIDOffsetListName}[] = {{");

                for (int i = 0; i < cmap.GlyphIDOffsetList.Count; i += 16)
                {
                    var chunk = cmap.GlyphIDOffsetList
                        .Skip(i)
                        .Take(16)
                        .Select(o => o.ToString())
                        .ToList();

                    cMapSection.Append(_Tab1);
                    cMapSection.AppendLine(string.Join(", ", chunk) + (i + 16 >= cmap.GlyphIDOffsetList.Count ? "" : ","));
                }

                cMapSection.AppendLine("};");
                cMapSection.AppendLine();
            }

        }

        // --- Generate cmap array ---
        cMapSection.AppendLine("/*Collect the unicode lists and glyph_id offsets*/");
        cMapSection.AppendLine($"static const lv_font_fmt_txt_cmap_t {_CMapsArrayName}[] = {{");

        foreach (var cmap in cmaps)
        {
            cMapSection.AppendLine($"{_Tab1}{{");
            cMapSection.Append($"{_Tab2}.range_start = 0x{cmap.RangeStart:X}, ");
            cMapSection.Append($".range_length = {cmap.RangeLength}, ");
            cMapSection.AppendLine($".glyph_id_start = {cmap.GlyphIDStart},");
            cMapSection.Append($"{_Tab2}.unicode_list = {(cmap.UnicodeList.Count > 0 ? cmap.UnicodeListName : "NULL")}, ");
            cMapSection.Append($".glyph_id_ofs_list = {(cmap.GlyphIDOffsetList.Count > 0 ? cmap.GlyphIDOffsetListName : "NULL")}, ");
            cMapSection.Append($".list_length = {(cmap.UnicodeList.Count > 0 ? cmap.UnicodeList.Count : cmap.GlyphIDOffsetList.Count)}, ");
            cMapSection.AppendLine($".type = {cmap.Type}");
            cMapSection.AppendLine($"{_Tab1}}},");
        }

        cMapSection.AppendLine("};");
        return cMapSection;
    }

    private static List<UnicodeCharacter> GetGlyphsCodePoints(IList<LVGLGlyph> glyphsToExport)
    {
        List<UnicodeCharacter> codePoints = new();
        foreach (var glyph in glyphsToExport)
        {
            if (glyph.CodePoints.Count > 0)
            {
                codePoints.AddRange(glyph.CodePoints.Values);
            }
        }
        return codePoints;
    }

    public static List<LVGLCMapRange> GenerateCMapRanges(List<UnicodeCharacter> characters)
    {
        var sorted = characters
            .Where(x => x.GlyphID.HasValue)
            .OrderBy(x => x.CodePoint)
            .ToList();

        var blocks = SplitForCMapSubtables(sorted);
        return blocks.Select(BuildCMapSubtable).ToList();
    }

    public static List<List<UnicodeCharacter>> SplitForCMapSubtables(List<UnicodeCharacter> sortedList)
    {
        const int maxRangeLength = 256;
        const int maxItems = 256;

        var result = new List<List<UnicodeCharacter>>();
        var current = new List<UnicodeCharacter>();

        uint? firstCodePoint = null;

        foreach (var u in sortedList)
        {
            if (u.GlyphID == null)
                continue;

            if (current.Count == 0)
            {
                current.Add(u);
                firstCodePoint = u.CodePoint;
                continue;
            }

            bool rangeTooLong = (u.CodePoint - firstCodePoint) >= maxRangeLength;
            bool tooManyItems = current.Count >= maxItems;

            if (rangeTooLong || tooManyItems)
            {
                result.Add(new List<UnicodeCharacter>(current));
                current.Clear();
                current.Add(u);
                firstCodePoint = u.CodePoint;
            }
            else
            {
                current.Add(u);
            }
        }

        if (current.Count > 0)
            result.Add(current);

        return result;
    }

    public static LVGLCMapRange BuildCMapSubtable(List<UnicodeCharacter> block)
    {
        var cmap = new LVGLCMapRange();

        var rangeStart = (int)block.First().CodePoint;
        var rangeEnd = (int)block.Last().CodePoint;
        var glyphStart = block.Min(x => x.GlyphID!.Value);

        cmap.RangeStart = rangeStart;
        cmap.RangeLength = rangeEnd - rangeStart + 1;
        cmap.GlyphIDStart = glyphStart;

        bool cpSequential = true;
        bool gidSequential = true;
        bool isDirectMap = true;

        for (int i = 0; i < block.Count; i++)
        {
            int expectedCP = rangeStart + i;
            int expectedGID = glyphStart + i;
            int actualCP = (int)block[i].CodePoint;
            int actualGID = block[i].GlyphID!.Value;

            if (actualCP != expectedCP) cpSequential = false;
            if (actualGID != expectedGID) gidSequential = false;
            if ((actualGID - glyphStart) != (actualCP - rangeStart))
                isDirectMap = false;
        }

        if (cpSequential && gidSequential && isDirectMap)
        {
            cmap.Type = LVGL_CMAP_TYPE.LV_FONT_FMT_TXT_CMAP_FORMAT0_TINY;
        }
        else if (gidSequential)
        {
            cmap.Type = LVGL_CMAP_TYPE.LV_FONT_FMT_TXT_CMAP_SPARSE_TINY;
            cmap.UnicodeList = block.Select(x => x.CodePoint - (uint)rangeStart).ToList();
            cmap.UnicodeListName = $"unicode_list_{rangeStart:X}";
            cmap.ListLength = cmap.UnicodeList.Count;
        }
        else if (cpSequential)
        {
            cmap.Type = LVGL_CMAP_TYPE.LV_FONT_FMT_TXT_CMAP_FORMAT0_FULL;
            cmap.GlyphIDOffsetList = block.Select(x => x.GlyphID!.Value - glyphStart).ToList();
            cmap.GlyphIDOffsetListName = $"glyph_id_ofs_list_{rangeStart:X}";
            cmap.ListLength = cmap.GlyphIDOffsetList.Count;
        }
        else
        {
            cmap.Type = LVGL_CMAP_TYPE.LV_FONT_FMT_TXT_CMAP_SPARSE_FULL;
            cmap.UnicodeList = block.Select(x => x.CodePoint - (uint)rangeStart).ToList();
            cmap.GlyphIDOffsetList = block.Select(x => x.GlyphID!.Value - glyphStart).ToList();
            cmap.UnicodeListName = $"unicode_list_{rangeStart:X}";
            cmap.GlyphIDOffsetListName = $"glyph_id_ofs_list_{rangeStart:X}";
            cmap.ListLength = cmap.UnicodeList.Count;
        }

        return cmap;
    }

    private static StringBuilder KerningSection(LVGLKerningClassResult kernResult)
    {
        StringBuilder kerningSection = new();
        kerningSection.AppendLine("/*-----------------");
        kerningSection.AppendLine("*    KERNING");
        kerningSection.AppendLine("*----------------*/");
        kerningSection.AppendLine();

        // --- kern_left_class_mapping ---
        kerningSection.AppendLine("/* Map glyph_ids to kern left classes */");      
        kerningSection.AppendLine($"static const uint8_t {_KerningLeftMapName}[] = {{");
        AppendByteArray(kerningSection, kernResult.LeftClassMap, 16, _Tab1);
        kerningSection.AppendLine("};");
        kerningSection.AppendLine();

        // --- kern_right_class_mapping ---
        kerningSection.AppendLine("/* Map glyph_ids to kern right classes */");
        kerningSection.AppendLine($"static const uint8_t {_KerningRightMapName}[] = {{");
        AppendByteArray(kerningSection, kernResult.RightClassMap, 16, _Tab1);
        kerningSection.AppendLine("};");
        kerningSection.AppendLine();

        // --- kern_class_values ---
        kerningSection.AppendLine("/* Kern values between classes */");
        kerningSection.AppendLine($"static const int8_t {_KerningValuesName}[] = {{");
        AppendSByteArray(kerningSection, kernResult.ClassValues, 16, "    ");
        kerningSection.AppendLine("};");
        kerningSection.AppendLine();

        // --- lv_font_fmt_txt_kern_classes_t structure ---
        kerningSection.AppendLine($"static const lv_font_fmt_txt_kern_classes_t {_KernClassesName} = {{");
        kerningSection.AppendLine($"{_Tab1}.class_pair_values   = {_KerningValuesName},");
        kerningSection.AppendLine($"{_Tab1}.left_class_mapping  = {_KerningLeftMapName},");
        kerningSection.AppendLine($"{_Tab1}.right_class_mapping = {_KerningRightMapName},");
        kerningSection.AppendLine($"{_Tab1}.left_class_cnt      = {kernResult.LeftClassCount},");
        kerningSection.AppendLine($"{_Tab1}.right_class_cnt     = {kernResult.RightClassCount},");
        kerningSection.AppendLine("};");
        kerningSection.AppendLine();

        return kerningSection;
    }

    public static List<KernPair> CollectUniqueKernPairs(List<LVGLGlyph> glyphs)
    {
        var set = new HashSet<(ushort, ushort)>();
        var result = new List<KernPair>();

        foreach (var glyph in glyphs)
        {
            foreach (var kp in glyph.LeftKernings)
            {
                if (set.Add((kp.Left, kp.Right)))
                    result.Add(kp);
            }

            foreach (var kp in glyph.RightKernings)
            {
                if (set.Add((kp.Left, kp.Right)))
                    result.Add(kp);
            }
        }

        return result;
    }

    public static int ScaleKernPairsInPlace(List<KernPair> pairs)
    {
        int maxAbs = pairs
            .Select(p => Math.Abs((int)p.Value))
            .DefaultIfEmpty(0)
            .Max();

        if (maxAbs <= 127)
            return 16; // No scaling needed

        double scaleFactor = 127.0 / maxAbs;
        int kernScale = (int)Math.Round(16.0 / scaleFactor);

        foreach (var p in pairs)
        {
            p.Value = (short)Math.Round(p.Value * scaleFactor);
        }

        return kernScale;
    }

    public static bool ShouldUseClassBasedKerning(List<LVGLGlyph> glyphs)
    {
        var pairs = CollectUniqueKernPairs(glyphs);
        int pairCount = pairs.Count;
        int glyphCount = glyphs.Count;

        if (pairCount > glyphCount * 2) return true;

        var sigs = new HashSet<string>();

        foreach (var g in glyphs)
        {
            var left = string.Join(",", g.LeftKernings.OrderBy(k => k.Right).Select(k => k.Value));
            var right = string.Join(",", g.RightKernings.OrderBy(k => k.Left).Select(k => k.Value));
            sigs.Add($"{left}|{right}");
        }

        int classCandidates = sigs.Count;

        return classCandidates < glyphCount * 0.5;
    }

    public static Dictionary<int, int> BuildKerningClassMap(
    List<LVGLGlyph> glyphs,
    bool isLeft)
    {
        var signatureToClass = new Dictionary<string, int>();
        var glyphIdToClass = new Dictionary<int, int>();
        int classCounter = 1;

        foreach (var glyph in glyphs.OrderBy(g => g.Index))
        {
            var kernList = isLeft ? glyph.LeftKernings : glyph.RightKernings;

            var signature = string.Join(",",
                kernList.OrderBy(k => isLeft ? k.Right : k.Left)
                        .Select(k => $"{(isLeft ? k.Right : k.Left)}:{k.Value}")
            );

            if (!signatureToClass.TryGetValue(signature, out int classId))
            {
                classId = classCounter++;
                signatureToClass[signature] = classId;
            }

            glyphIdToClass[glyph.Index] = classId;
        }

        return glyphIdToClass;
    }

    public static List<byte> BuildClassMapping(List<LVGLGlyph> glyphs, Dictionary<int, int> glyphToClass)
    {
        int maxId = glyphs.Max(g => g.Index);
        var mapping = new byte[maxId + 1];

        foreach (var glyph in glyphs)
        {
            mapping[glyph.Index] = (byte)(glyphToClass.TryGetValue(glyph.Index, out var classId) ? classId : 0);
        }

        return mapping.ToList();
    }

    public static sbyte[] BuildClassPairValues(
    Dictionary<int, int> leftMap,
    Dictionary<int, int> rightMap,
    List<KernPair> pairs,
    out int leftClassCount,
    out int rightClassCount)
    {
        leftClassCount = leftMap.Values.Max();
        rightClassCount = rightMap.Values.Max();
        var table = new sbyte[leftClassCount * rightClassCount];

        foreach (var pair in pairs)
        {
            if (!leftMap.TryGetValue(pair.Left, out int leftClass) || leftClass == 0) continue;
            if (!rightMap.TryGetValue(pair.Right, out int rightClass) || rightClass == 0) continue;

            int index = (leftClass - 1) * rightClassCount + (rightClass - 1);
            table[index] = (sbyte)pair.Value;
        }

        return table;
    }

    public class LVGLKerningClassResult
    {
        public List<byte> LeftClassMap { get; set; } = [];
        public List<byte> RightClassMap { get; set; } = [];
        public sbyte[] ClassValues { get; set; } = [];
        public int LeftClassCount { get; set; }
        public int RightClassCount { get; set; }
    }

    public static LVGLKerningClassResult GenerateKerningClassTables(List<LVGLGlyph> glyphs, List<KernPair> pairs)
    {


        var leftMap = BuildKerningClassMap(glyphs, isLeft: true);
        var rightMap = BuildKerningClassMap(glyphs, isLeft: false);

        var leftClassMap = BuildClassMapping(glyphs, leftMap);
        var rightClassMap = BuildClassMapping(glyphs, rightMap);

        var classValues = BuildClassPairValues(leftMap, rightMap, pairs,
            out int leftClassCount,
            out int rightClassCount);

        return new LVGLKerningClassResult
        {
            LeftClassMap = leftClassMap,
            RightClassMap = rightClassMap,
            ClassValues = classValues,
            LeftClassCount = leftClassCount,
            RightClassCount = rightClassCount
        };
    }

    private static void AppendByteArray(StringBuilder sb, List<byte> values, int perLine, string indent)
    {
        for (int i = 0; i < values.Count; i += perLine)
        {
            var line = values.Skip(i).Take(perLine).Select(v => $"{v}");
            sb.Append(indent);
            sb.AppendLine(string.Join(", ", line) + (i + perLine < values.Count ? "," : ""));
        }
    }

    private static void AppendSByteArray(StringBuilder sb, sbyte[] values, int perLine, string indent)
    {
        for (int i = 0; i < values.Length; i += perLine)
        {
            var line = values.Skip(i).Take(perLine).Select(v => $"{v}");
            sb.Append(indent);
            sb.AppendLine(string.Join(", ", line) + (i + perLine < values.Length ? "," : ""));
        }
    }



    private static StringBuilder FooterSection(LVGLFont lvglFont, bool haveKernings, int kerningScale, int cMapsCount, int kernClassesCount)
    {
        StringBuilder footerSection = new();
        footerSection.AppendLine("/*--------------------");
        footerSection.AppendLine(" *  ALL CUSTOM DATA");
        footerSection.AppendLine("*--------------------*/");
        footerSection.AppendLine();
        footerSection.AppendLine("#if LVGL_VERSION_MAJOR >= 8");
        footerSection.AppendLine("/*Store all the custom data of the font*/");
        footerSection.AppendLine();
        footerSection.AppendLine("static const lv_font_fmt_txt_dsc_t font_dsc = {");
        footerSection.AppendLine("#else");
        footerSection.AppendLine($"static lv_font_fmt_txt_dsc_t {_FontDescriptorName} = {{");
        footerSection.AppendLine("#endif");
        footerSection.AppendLine($"{_Tab1}.glyph_bitmap = {_BitmapArrayName},");
        footerSection.AppendLine($"{_Tab1}.glyph_dsc = {_DescriptorArrayName},");
        footerSection.AppendLine($"{_Tab1}.cmaps = {_CMapsArrayName},");
        footerSection.AppendLine($"{_Tab1}.kern_dsc = {(haveKernings ? "&" + _KernClassesName : "NULL")},");
        footerSection.AppendLine($"{_Tab1}.kern_scale = {(haveKernings ? kerningScale : 0)},");
        footerSection.AppendLine($"{_Tab1}.cmap_num = {cMapsCount},");
        footerSection.AppendLine($"{_Tab1}.bpp = {(int)lvglFont.FontSettings.FontBitPerPixel},");
        footerSection.AppendLine($"{_Tab1}.kern_classes = {kernClassesCount},");
        footerSection.AppendLine($"{_Tab1}.bitmap_format = 0,");
        footerSection.AppendLine("};");
        footerSection.AppendLine();
        footerSection.AppendLine("/*-----------------");
        footerSection.AppendLine("*  PUBLIC FONT");
        footerSection.AppendLine("*----------------*/");
        footerSection.AppendLine();
        footerSection.AppendLine("/*Initialize a public general font descriptor*/");
        footerSection.AppendLine("#if LVGL_VERSION_MAJOR >= 8");
        footerSection.AppendLine($"const lv_font_t {lvglFont.FontSettings.FontName} = {{");
        footerSection.AppendLine("#else");
        footerSection.AppendLine($"lv_font_t {lvglFont.FontSettings.FontName} = {{");
        footerSection.AppendLine("#endif");
        footerSection.AppendLine($"{_Tab1}.get_glyph_dsc = lv_font_get_glyph_dsc_fmt_txt,    /*Function pointer to get glyph's data*/");
        footerSection.AppendLine($"{_Tab1}.get_glyph_bitmap = lv_font_get_bitmap_fmt_txt,    /*Function pointer to get glyph's bitmap*/");
        footerSection.AppendLine($"{_Tab1}.line_height = {lvglFont.FontInformations.LineHeight},          /*The maximum line height required by the font*/");
        footerSection.AppendLine($"{_Tab1}.base_line = {lvglFont.FontInformations.BaseLine},             /*Baseline measured from the bottom of the line*/");
        footerSection.AppendLine("#if !(LVGL_VERSION_MAJOR == 6 && LVGL_VERSION_MINOR == 0)");
        footerSection.AppendLine($"{_Tab1}.subpx = LV_FONT_SUBPX_NONE,");
        footerSection.AppendLine("#endif");
        footerSection.AppendLine("#if LV_VERSION_CHECK(7, 4, 0) || LVGL_VERSION_MAJOR >= 8");
        footerSection.AppendLine($"{_Tab1}.underline_position = {lvglFont.FontInformations.UnderlinePosition},");
        footerSection.AppendLine($"{_Tab1}.underline_thickness = {lvglFont.FontInformations.UnderlineThickness},");
        footerSection.AppendLine("#endif");
        footerSection.AppendLine($"{_Tab1}.dsc = &{_FontDescriptorName}           /*The custom font data. Will be accessed by `get_glyph_bitmap/dsc` */");
        footerSection.AppendLine("};");
        footerSection.AppendLine();
        footerSection.AppendLine($"#endif /* #if CUSTOM_FONT_{lvglFont.FontSettings.FontName.ToUpper()} */");
        return footerSection;
    }
}
