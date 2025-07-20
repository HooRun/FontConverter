using FontConverter.SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FontConverter.SharedLibrary.Helpers;

public static class ExportKernHelper
{
    // --- کرنینگ فرمت 0: جفت گلیف با مقدار ---
    public static List<KernPair> CollectFormat0Pairs(List<LVGLGlyph> glyphs)
    {
        var pairs = new List<KernPair>();
        var seen = new HashSet<(ushort, ushort)>();

        foreach (var g in glyphs)
        {
            foreach (var kp in g.LeftKernings)
            {
                var key = (kp.Left, kp.Right);
                if (seen.Add(key))
                    pairs.Add(new KernPair
                    {
                        Left = kp.Left,
                        Right = kp.Right,
                        Value = kp.Value
                    });
            }
        }

        return pairs;
    }

    public static int ScalePairsInPlace(List<KernPair> pairs)
    {
        int maxAbs = pairs.Select(p => Math.Abs(p.Value)).DefaultIfEmpty().Max();

        if (maxAbs <= 127)
            return 16;

        double scaleFactor = 127.0 / maxAbs;
        int scale = (int)Math.Round(16.0 / scaleFactor);

        foreach (var p in pairs)
            p.Value = (short)Math.Round(p.Value * scaleFactor);

        return scale;
    }

    // --- کرنینگ فرمت 3: کلاس‌بندی‌شده ---
    public static LVGLKerningClassResult? CollectFormat3Data(List<LVGLGlyph> glyphs)
    {
        var pairs = CollectFormat0Pairs(glyphs);
        int scale = ScalePairsInPlace(pairs);

        var leftKerningMap = new Dictionary<int, Dictionary<int, short>>();
        var rightKerningMap = new Dictionary<int, Dictionary<int, short>>();

        foreach (var g in glyphs)
        {
            if (g.LeftKernings.Count > 0)
                leftKerningMap[g.Index] = g.LeftKernings.ToDictionary(k => (int)k.Right, k => k.Value);

            foreach (var kp in g.LeftKernings)
            {
                if (!rightKerningMap.TryGetValue(kp.Right, out var dict))
                    rightKerningMap[kp.Right] = dict = new Dictionary<int, short>();

                dict[g.Index] = kp.Value;
            }
        }

        var leftClasses = BuildKerningClasses(leftKerningMap);
        var rightClasses = BuildKerningClasses(rightKerningMap);

        if (leftClasses.Count >= 255 || rightClasses.Count >= 255)
            return null;

        var leftMap = BuildClassMapping(glyphs.Count, leftClasses);
        var rightMap = BuildClassMapping(glyphs.Count, rightClasses);
        var classValues = BuildClassValueTable(leftClasses, rightClasses, leftKerningMap);

        return new LVGLKerningClassResult
        {
            LeftClassCount = leftClasses.Count,
            RightClassCount = rightClasses.Count,
            LeftClassMap = leftMap,
            RightClassMap = rightMap,
            ClassValues = classValues,
            Scale = scale
        };
    }

    // --- تشخیص فرمت بهینه ---
    public static bool ShouldUseFormat3Auto(List<LVGLGlyph> glyphs)
    {
        if (glyphs == null || glyphs.Count == 0)
            return false;

        var pairCount = glyphs.SelectMany(g => g.LeftKernings)
                              .Select(k => (k.Left, k.Right))
                              .Distinct()
                              .Count();

        int glyphCount = glyphs.Count;

        if (pairCount > glyphCount * 2)
            return true;

        var signatures = new HashSet<string>();
        foreach (var g in glyphs)
        {
            string left = string.Join(",", g.LeftKernings.OrderBy(k => k.Right).Select(k => k.Value));
            string right = string.Join(",", g.RightKernings.OrderBy(k => k.Left).Select(k => k.Value));
            signatures.Add($"{left}|{right}");
        }

        return signatures.Count < glyphCount * 0.5;
    }

    // --- ساخت کلاس‌بندی ---
    private static Dictionary<string, List<int>> BuildKerningClasses(Dictionary<int, Dictionary<int, short>> kerningMap)
    {
        var result = new Dictionary<string, List<int>>();

        foreach (var kv in kerningMap)
        {
            var serialized = string.Join(",", kv.Value.OrderBy(p => p.Key).Select(p => $"{p.Key}:{p.Value}"));

            if (!result.TryGetValue(serialized, out var list))
                result[serialized] = list = new List<int>();

            list.Add(kv.Key);
        }

        return result;
    }

    private static List<byte> BuildClassMapping(int maxGlyphId, Dictionary<string, List<int>> classes)
    {
        var mapping = new byte[maxGlyphId + 1];
        int classId = 1;

        foreach (var group in classes.Values)
        {
            foreach (var glyphId in group)
            {
                if (glyphId < mapping.Length)
                    mapping[glyphId] = (byte)classId;
            }
            classId++;
        }

        return mapping.ToList();
    }

    private static sbyte[] BuildClassValueTable(
        Dictionary<string, List<int>> leftClasses,
        Dictionary<string, List<int>> rightClasses,
        Dictionary<int, Dictionary<int, short>> leftKerningMap)
    {
        int leftCount = leftClasses.Count;
        int rightCount = rightClasses.Count;

        var table = new sbyte[leftCount * rightCount];
        var leftList = leftClasses.Values.ToList();
        var rightList = rightClasses.Values.ToList();

        for (int i = 0; i < leftCount; i++)
        {
            int leftGlyph = leftList[i][0];

            for (int j = 0; j < rightCount; j++)
            {
                int rightGlyph = rightList[j][0];

                short value = 0;
                if (leftKerningMap.TryGetValue(leftGlyph, out var dict))
                    dict.TryGetValue(rightGlyph, out value);

                table[i * rightCount + j] = (sbyte)value;
            }
        }

        return table;
    }
}
