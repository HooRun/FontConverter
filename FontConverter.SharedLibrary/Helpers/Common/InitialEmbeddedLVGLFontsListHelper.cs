using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FontConverter.SharedLibrary.Helpers;

public static class InitialEmbeddedLVGLFontsListHelper
{
    private static readonly SortedList<int, string> _EmbeddedLVGLFontList = new()
    {
        {0, "lv_font_montserrat_8"},
        {1, "lv_font_montserrat_10"},
        {2, "lv_font_montserrat_12"},
        {3, "lv_font_montserrat_14"},
        {4, "lv_font_montserrat_16"},
        {5, "lv_font_montserrat_18"},
        {6, "lv_font_montserrat_20"},
        {7, "lv_font_montserrat_22"},
        {8, "lv_font_montserrat_24"},
        {9, "lv_font_montserrat_26"},
        {10, "lv_font_montserrat_28"},
        {11, "lv_font_montserrat_28_compressed"},
        {12, "lv_font_montserrat_30"},
        {13, "lv_font_montserrat_32"},
        {14, "lv_font_montserrat_34"},
        {15, "lv_font_montserrat_36"},
        {16, "lv_font_montserrat_38"},
        {17, "lv_font_montserrat_40"},
        {18, "lv_font_montserrat_42"},
        {19, "lv_font_montserrat_44"},
        {20, "lv_font_montserrat_46"},
        {21, "lv_font_montserrat_48"},
        {22, "lv_font_dejavu_16_persian_hebrew"},
        {23, "lv_font_simsun_14_cjk"},
        {24, "lv_font_simsun_16_cjk"},
        {25, "lv_font_source_han_sans_sc_14_cjk"},
        {26, "lv_font_source_han_sans_sc_16_cjk"},
        {27, "lv_font_unscii_8"},
        {28, "lv_font_unscii_16"},
    };

    public static SortedList<int, string> InitialEmbeddedLVGLFontsList()
    {
        return _EmbeddedLVGLFontList;
    }
}
