using FontConverter.SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FontConverter.SharedLibrary.Helpers;

public static class ExportToSymbolHelper
{
    private const string _Tab1 = "    ";
    private const string _Tab2 = "        ";
    private const string _Tab3 = "            ";
    private const string _Tab4 = "                ";

    public static async Task<string> ExportToSymbol(LVGLFont lvglFont, IList<LVGLGlyph> glyphsToExport)
    {
        await Task.Yield();
        try
        {
            if (lvglFont == null || glyphsToExport == null || glyphsToExport.Count <= 0)
                return string.Empty;

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

            StringBuilder symbols = new();

            symbols.AppendLine("/*******************************************************************************");
            symbols.AppendLine("* Font Converter For LVGL");
            symbols.AppendLine("* https://hoorun.github.io/FontConverter/");
            symbols.AppendLine("*");
            symbols.AppendFormat("* OpenType Font Name: {0}", lvglFont.FontInformations.FontName).AppendLine();
            symbols.AppendFormat("* Font Name: {0}", lvglFont.FontSettings.FontName).AppendLine();
            symbols.AppendFormat("* Font BPP: {0}", lvglFont.FontSettings.FontBitPerPixel).AppendLine();
            symbols.AppendFormat("* Font Size: {0}", lvglFont.FontSettings.FontSize).AppendLine();
            symbols.AppendFormat("* Font Adjusments: AntiAlias={0}, Dither={1}, Style={2}, StrokeWidth={3}, Gamma={4}, Threshold={5}",
                lvglFont.FontAdjusments.AntiAlias.ToString(),
                lvglFont.FontAdjusments.Dither.ToString(),
                lvglFont.FontAdjusments.Style.ToString(),
                lvglFont.FontAdjusments.StrokeWidth,
                gamma,
                lvglFont.FontAdjusments.Threshold).AppendLine();
            symbols.AppendLine("******************************************************************************/");
            symbols.AppendLine();
            symbols.AppendLine($"#ifndef CUSTOM_FONT_{lvglFont.FontSettings.FontName.ToUpper()}_SYMBOL_DEF_H");
            symbols.AppendLine($"#define CUSTOM_FONT_{lvglFont.FontSettings.FontName.ToUpper()}_SYMBOL_DEF_H");
            symbols.AppendLine();
            symbols.AppendLine("#ifdef __cplusplus");
            symbols.AppendLine($"{_Tab1}extern \"C\" {{");
            symbols.AppendLine("#endif");
            symbols.AppendLine();
            symbols.AppendLine($"#ifndef CUSTOM_FONT_{lvglFont.FontSettings.FontName.ToUpper()}_SYMBOL");
            symbols.AppendLine($"{_Tab1}#define CUSTOM_FONT_{lvglFont.FontSettings.FontName.ToUpper()}_SYMBOL 1");
            symbols.AppendLine("#endif");
            symbols.AppendLine();
            symbols.AppendLine($"#if CUSTOM_FONT_{lvglFont.FontSettings.FontName.ToUpper()}_SYMBOL");
            symbols.AppendLine();

            symbols.AppendLine("#ifndef SYMBOLDEF_STRUCT_DEFINED");
            symbols.AppendLine($"{_Tab1}#define SYMBOLDEF_STRUCT_DEFINED");
            symbols.AppendLine();
            symbols.AppendLine($"{_Tab1}typedef struct {{");
            symbols.AppendLine($"{_Tab2}const char* name;");
            symbols.AppendLine($"{_Tab2}const char* value;");
            symbols.AppendLine($"{_Tab1}}} symbol_def_t;");
            symbols.AppendLine("#endif /* #ifndef SYMBOLDEF_STRUCT_DEFINED */");
            symbols.AppendLine();


            string fontName = lvglFont.FontSettings.FontName.ToUpper();

            List<string> symbolNames = [];
            List<string> symbolValues = [];
            List<string> listNames = [];
            List<string> comments = [];

            List<string> glyphIds = [];
            List<string> glyphNames = [];
            List<string> glyphUCs = [];
            List<string> glyphUCHexs = [];

            int maxSymbolName = 0;
            int maxValueName = 0;
            int maxListName = 0;

            int maxGlyphIdLenght = 0;
            int maxGlyphNameLenght = 0;
            int maxGlyphUniLenght = 0;
            int maxGlyphUniHexLenght = 0;

            int totalSymbolsCount = 0;

            foreach (var glyph in glyphsToExport)
            {
                string name = SanitizeDefineName(glyph.Name);
                if (string.IsNullOrEmpty(name))
                {
                    name = $"glyph_{glyph.Index}";
                }

                int codePoint = 0;
                if (glyph.CodePoints.Count > 0)
                {
                    var cp = glyph.CodePoints.Values.FirstOrDefault()?.CodePoint;
                    if (cp != null)
                        codePoint = (int)cp;
                }

                string symbolName = $"{fontName}_SYMBOL_{name.ToUpper()}";
                string symbolValue = $"\"{UnicodeToUtf8Escaped(codePoint)}\"";
                string listName = $"\"{glyph.Name}\"";

                symbolNames.Add(symbolName);
                symbolValues.Add(symbolValue);
                listNames.Add(listName);

                if (symbolName.Length > maxSymbolName)
                    maxSymbolName = symbolName.Length;

                if (symbolValue.Length > maxValueName)
                    maxValueName = symbolValue.Length;

                if (listName.Length > maxListName)
                    maxListName = listName.Length;

                string id = glyph.Index.ToString();
                string uc = codePoint.ToString();
                string uchex = $"U+{codePoint:X6}";

                glyphIds.Add(id);
                glyphNames.Add(glyph.Name);
                glyphUCs.Add(uc);
                glyphUCHexs.Add(uchex);

                if (id.Length > maxGlyphIdLenght)
                    maxGlyphIdLenght = id.Length;

                if (glyph.Name.Length > maxGlyphNameLenght)
                    maxGlyphNameLenght = glyph.Name.Length;

                if (uc.Length > maxGlyphUniLenght)
                    maxGlyphUniLenght = uc.Length;

                if (uchex.Length > maxGlyphUniHexLenght)
                    maxGlyphUniHexLenght = uchex.Length;

                totalSymbolsCount++;
            }


            for (int si = 0; si < totalSymbolsCount; si++)
            {
                StringBuilder comment = new();
                comment.AppendFormat("/* Id: {0},{1}Name: {2},{3}CodePoint: {4},{5}Unicode: {6}{7} */",
                    glyphIds[si],
                    GenSpace(maxGlyphIdLenght - glyphIds[si].Length),
                    glyphNames[si],
                    GenSpace(maxGlyphNameLenght - glyphNames[si].Length),
                    glyphUCs[si],
                    GenSpace(maxGlyphUniLenght - glyphUCs[si].Length),
                    glyphUCHexs[si],
                    GenSpace(maxGlyphUniHexLenght - glyphUCHexs[si].Length)
                    );
                comments.Add(comment.ToString());

                symbols.AppendFormat("#define {0}{1}{2}{3}",
                    symbolNames[si],
                    GenSpace(maxSymbolName - symbolNames[si].Length),
                    symbolValues[si],
                    GenSpace(maxValueName - symbolValues[si].Length)
                    );
                symbols.Append(comment);
                symbols.AppendLine();
            }
            symbols.AppendLine();


            string symbolDefMacroName = $"{fontName}_DEFINE_SYMBOL";
            symbols.AppendLine($"#define {symbolDefMacroName}(name, value) {{ name, value }},");
            symbols.AppendLine();
            symbols.AppendLine($"#define {fontName}_SYMBOLS_LIST \\");
            for (int si = 0; si < (totalSymbolsCount - 1); si++)
            {
                symbols.AppendFormat("{0}{1}({2},{3}{4}){5}{6} \\",
                    _Tab1,
                    symbolDefMacroName,
                    listNames[si],
                    GenSpace(maxListName - listNames[si].Length),
                    symbolValues[si],
                    GenSpace(maxValueName - symbolValues[si].Length),
                    comments[si]);
                symbols.AppendLine();
            }
            symbols.AppendFormat("{0}{1}({2},{3}{4}){5}{6}",
                    _Tab1,
                    symbolDefMacroName,
                    listNames[totalSymbolsCount - 1],
                    GenSpace(maxListName - listNames[totalSymbolsCount - 1].Length),
                    symbolValues[totalSymbolsCount - 1],
                    GenSpace(maxValueName - symbolValues[totalSymbolsCount - 1].Length),
                    comments[totalSymbolsCount - 1]);
            symbols.AppendLine();
            symbols.AppendLine();

            symbols.AppendLine($"{_Tab1}static const symbol_def_t {fontName.ToLower()}_symbol_table[] = {{");
            symbols.AppendLine($"{_Tab2}{fontName}_SYMBOLS_LIST");
            symbols.AppendLine($"{_Tab2}{{NULL,NULL}}");
            symbols.AppendLine($"{_Tab1}}};");
            symbols.AppendLine($"#undef {symbolDefMacroName}");
            symbols.AppendLine();
            symbols.AppendLine($"#define TOTAL_{fontName}_SYMBOLS    {totalSymbolsCount}");
            symbols.AppendLine();
            symbols.AppendLine($"#endif /* #if CUSTOM_FONT_{fontName}_SYMBOL */");
            symbols.AppendLine();
            symbols.AppendLine("#ifdef __cplusplus");
            symbols.AppendLine($"{_Tab1}}} /* extern \"C\" */");
            symbols.AppendLine("#endif");
            symbols.AppendLine();
            symbols.AppendLine($"#endif /* #if CUSTOM_FONT_{fontName}_SYMBOL_DEF_H */");
            return symbols.ToString();
        }
        catch (Exception ex)
        {
            return string.Empty;
        }
    }

    public static string UnicodeToUtf8Escaped(int codepoint)
    {
        try
        {
            string unicode = char.ConvertFromUtf32(codepoint);
            byte[] utf8Bytes = Encoding.UTF8.GetBytes(unicode);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in utf8Bytes)
            {
                sb.AppendFormat("\\x{0:X2}", b);
            }
            return sb.ToString();
        }
        catch (Exception ex)
        {
            return string.Empty;
        }
    }

    public static string SanitizeDefineName(string name)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name))
                return string.Empty;

            var sb = new StringBuilder();

            foreach (char c in name)
            {
                if (char.IsLetterOrDigit(c) || c == '_')
                    sb.Append(c);
                else
                    sb.Append('_');
            }

            string result = sb.ToString();

            result = result.TrimStart('_');

            return result;
        }
        catch (Exception ex)
        {
            return string.Empty;
        }
    }

    private static string GenSpace(int count)
    {
        if (count <= 0)
            return " ";
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < (count + 1); i++)
        {
            sb.Append(' ');
        }
        return sb.ToString();
    }

}
