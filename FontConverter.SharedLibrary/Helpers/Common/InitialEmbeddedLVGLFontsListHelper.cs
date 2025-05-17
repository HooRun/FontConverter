using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FontConverter.SharedLibrary.Helpers;

public static class InitialEmbeddedLVGLFontsListHelper
{
    public static async Task<SortedList<int, string>> InitialEmbeddedLVGLFontsList(CancellationToken cancellationToken = default)
    {
        SortedList<int, string> embeddedLVGLFontList = new();
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                embeddedLVGLFontList.Add(0, "lv_font_montserrat_8");
                embeddedLVGLFontList.Add(1, "lv_font_montserrat_10");
                embeddedLVGLFontList.Add(2, "lv_font_montserrat_12");
                embeddedLVGLFontList.Add(3, "lv_font_montserrat_14");
                embeddedLVGLFontList.Add(4, "lv_font_montserrat_16");
                embeddedLVGLFontList.Add(5, "lv_font_montserrat_18");
                embeddedLVGLFontList.Add(6, "lv_font_montserrat_20");
                embeddedLVGLFontList.Add(7, "lv_font_montserrat_22");
                embeddedLVGLFontList.Add(8, "lv_font_montserrat_24");
                embeddedLVGLFontList.Add(9, "lv_font_montserrat_26");
                embeddedLVGLFontList.Add(10, "lv_font_montserrat_28");
                embeddedLVGLFontList.Add(11, "lv_font_montserrat_28_compressed");
                embeddedLVGLFontList.Add(12, "lv_font_montserrat_30");
                embeddedLVGLFontList.Add(13, "lv_font_montserrat_32");
                embeddedLVGLFontList.Add(14, "lv_font_montserrat_34");
                embeddedLVGLFontList.Add(15, "lv_font_montserrat_36");
                embeddedLVGLFontList.Add(16, "lv_font_montserrat_38");
                embeddedLVGLFontList.Add(17, "lv_font_montserrat_40");
                embeddedLVGLFontList.Add(18, "lv_font_montserrat_42");
                embeddedLVGLFontList.Add(19, "lv_font_montserrat_44");
                embeddedLVGLFontList.Add(20, "lv_font_montserrat_46");
                embeddedLVGLFontList.Add(21, "lv_font_montserrat_48");
                embeddedLVGLFontList.Add(22, "lv_font_dejavu_16_persian_hebrew");
                embeddedLVGLFontList.Add(23, "lv_font_simsun_14_cjk");
                embeddedLVGLFontList.Add(24, "lv_font_simsun_16_cjk");
                embeddedLVGLFontList.Add(25, "lv_font_source_han_sans_sc_14_cjk");
                embeddedLVGLFontList.Add(26, "lv_font_source_han_sans_sc_16_cjk");
                embeddedLVGLFontList.Add(27, "lv_font_unscii_8");
                embeddedLVGLFontList.Add(28, "lv_font_unscii_16");
            }, cancellationToken);

        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            throw;
        }
        return embeddedLVGLFontList;
    }
}
