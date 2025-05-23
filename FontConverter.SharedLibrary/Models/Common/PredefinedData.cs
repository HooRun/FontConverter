using FontConverter.SharedLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FontConverter.SharedLibrary.Models;

public class PredefinedData
{
    public PredefinedData()
    {
        UnicodeBlockCollection = new();
        StandardMacintoshGlyphNames = new();
        BitPerPixelList = new();
        SubPixelList = new();
        GlyphStyleList = new();
        EmbeddedLVGLFontsList = new();
    }

    public UnicodeBlockCollection UnicodeBlockCollection { get; set; }
    public SortedList<int, string> StandardMacintoshGlyphNames { get; set; }
    public SortedList<LVGLFontEnums.BIT_PER_PIXEL_ENUM, string> BitPerPixelList { get; set; }
    public SortedList<LVGLFontEnums.SUB_Pixel_ENUM, string> SubPixelList { get; set; }
    public SortedList<LVGLFontEnums.GLYPH_STYLE, string> GlyphStyleList { get; set; }
    public SortedList<int, string> EmbeddedLVGLFontsList { get; set; }
}
