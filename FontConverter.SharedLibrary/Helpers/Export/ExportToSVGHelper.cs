using FontConverter.SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FontConverter.SharedLibrary.Helpers;

public static class ExportToSVGHelper
{
    private const string _Tab1 = "    ";
    private const string _Tab2 = "        ";
    private const string _Tab3 = "            ";
    private const string _Tab4 = "                ";

    public static async Task<string> ExportToSVG(LVGLFont lvglFont, IList<LVGLGlyph> glyphsToExport)
    {
        await Task.Yield();

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
        string fontName = lvglFont.FontSettings.FontName.ToUpper();
        StringBuilder svg = new();

        svg.AppendLine("/*******************************************************************************");
        svg.AppendLine("* Font Converter For LVGL");
        svg.AppendLine("* https://hoorun.github.io/FontConverter/");
        svg.AppendLine("*");
        svg.AppendFormat("* OpenType Font Name: {0}", lvglFont.FontInformations.FontName).AppendLine();
        svg.AppendFormat("* Font Name: {0}", lvglFont.FontSettings.FontName).AppendLine();
        svg.AppendFormat("* Font BPP: {0}", lvglFont.FontSettings.FontBitPerPixel).AppendLine();
        svg.AppendFormat("* Font Size: {0}", lvglFont.FontSettings.FontSize).AppendLine();
        svg.AppendFormat("* Font Adjusments: AntiAlias={0}, Dither={1}, Style={2}, StrokeWidth={3}, Gamma={4}, Threshold={5}",
            lvglFont.FontAdjusments.AntiAlias.ToString(),
            lvglFont.FontAdjusments.Dither.ToString(),
            lvglFont.FontAdjusments.Style.ToString(),
            lvglFont.FontAdjusments.StrokeWidth,
            gamma,
            lvglFont.FontAdjusments.Threshold).AppendLine();
        svg.AppendLine("******************************************************************************/");
        svg.AppendLine();
        svg.AppendLine($"#ifndef CUSTOM_FONT_{fontName}_SVG_DEF_H");
        svg.AppendLine($"#define CUSTOM_FONT_{fontName}_SVG_DEF_H");
        svg.AppendLine();
        svg.AppendLine("#ifdef __cplusplus");
        svg.AppendLine($"{_Tab1}extern \"C\" {{");
        svg.AppendLine("#endif");
        svg.AppendLine();
        svg.AppendLine($"#ifndef CUSTOM_FONT_{fontName}_SVG");
        svg.AppendLine($"{_Tab1}#define CUSTOM_FONT_{fontName}_SVG 1");
        svg.AppendLine("#endif");
        svg.AppendLine();
        svg.AppendLine($"#if CUSTOM_FONT_{fontName}_SVG");
        svg.AppendLine();

        svg.AppendLine("#ifndef SVGDEF_STRUCT_DEFINED");
        svg.AppendLine($"{_Tab1}#define SVGDEF_STRUCT_DEFINED");
        svg.AppendLine();
        svg.AppendLine($"{_Tab1}typedef struct {{");
        svg.AppendLine($"{_Tab2}const int id;");
        svg.AppendLine($"{_Tab2}const char* name;");
        svg.AppendLine($"{_Tab2}const float width;");
        svg.AppendLine($"{_Tab2}const float height;");
        svg.AppendLine($"{_Tab2}const float offset_x;");
        svg.AppendLine($"{_Tab2}const float offset_y;");
        svg.AppendLine($"{_Tab2}const char* path;");
        svg.AppendLine($"{_Tab1}}} svg_def_t;");
        svg.AppendLine();
        svg.AppendLine($"{_Tab1}static const char svg_template[] =");
        svg.AppendLine($"{_Tab2}\"<svg width=\\\"%.02f\\\" height=\\\"%.02f\\\" viewBox=\\\"%.02f %.02f %.02f %.02f\\\" preserveAspectRatio=\\\"xMidYMid meet\\\">\"");
        svg.AppendLine($"{_Tab2}\"<g transform=\\\"translate(%.02f, %.02f)\\\">\"");
        svg.AppendLine($"{_Tab2}\"<path d=\\\"%s\\\" fill=\\\"%s\\\" fill-opacity=\\\"%.02f\\\"/>\"");
        svg.AppendLine($"{_Tab2}\"</g>\"");
        svg.AppendLine($"{_Tab2}\"</svg>\";");
        svg.AppendLine();
        svg.AppendLine($"{_Tab1}#define MAX_TEMPLATE_LENGTH    250");
        svg.AppendLine("#endif /* #ifndef SVGDEF_STRUCT_DEFINED */");
        svg.AppendLine();

        string svgDefMacroName = $"{fontName}_DEFINE_SVG";
        svg.AppendLine($"#define {svgDefMacroName}(id, name, width, height, offset_x, offset_y, path) {{ id, name, width, height, offset_x, offset_y, path }},");
        svg.AppendLine();
        svg.AppendLine($"#define {fontName}_SVGS_LIST \\");

        string listMacroneme = $"{_Tab1}{svgDefMacroName}(";

        int max_path_lenght = 0;
        int glyph_id = 0;
        foreach (var glyph in glyphsToExport)
        {
            svg.AppendLine($"{_Tab1}{svgDefMacroName}(\\");
            svg.AppendLine($"{_Tab2}{glyph_id}, \\");
            svg.AppendLine($"{_Tab2}\"{glyph.Name}\", \\");
            svg.AppendLine($"{_Tab2}{glyph.SVG.Width:F2}, \\");
            svg.AppendLine($"{_Tab2}{glyph.SVG.Height:F2}, \\");
            svg.AppendLine($"{_Tab2}{glyph.SVG.TranslateX:F2}, \\");
            svg.AppendLine($"{_Tab2}{glyph.SVG.TranslateY:F2}, \\");
            svg.AppendLine($"{_Tab2}\"{glyph.SVG.Path}\") \\");
            if (glyph.SVG.Path.Length > max_path_lenght)
            {
                max_path_lenght = glyph.SVG.Path.Length;
            }
            glyph_id++;
        }
        svg.AppendLine();
        svg.AppendLine();
        svg.AppendLine($"{_Tab1}static const svg_def_t {fontName.ToLower()}_svg_table[] = {{");
        svg.AppendLine($"{_Tab2}{fontName}_SVGS_LIST");
        svg.AppendLine($"{_Tab1}}};");
        svg.AppendLine($"#undef {svgDefMacroName}");

        svg.AppendLine();
        svg.AppendLine($"#define TOTAL_{fontName}_SVGS    {glyphsToExport.Count}");
        svg.AppendLine($"#define MAX_PATH_LENGTH_{fontName}    {max_path_lenght}");
        svg.AppendLine();
        svg.AppendLine($"#endif /* #if CUSTOM_FONT_{fontName}_SVG */");
        svg.AppendLine();
        svg.AppendLine("#ifdef __cplusplus");
        svg.AppendLine($"{_Tab1}}} /* extern \"C\" */");
        svg.AppendLine("#endif");
        svg.AppendLine();
        svg.AppendLine($"#endif /* #if CUSTOM_FONT_{fontName}_SVG_DEF_H */");

        return svg.ToString();
    }
}
