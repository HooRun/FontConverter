using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FontConverter.SharedLibrary.Helpers.LVGLFontEnums;

namespace FontConverter.SharedLibrary.Models;

public class LVGLCMapRange
{
    public LVGLCMapRange()
    {
        RangeStart = 0;
        RangeLength = 0;
        GlyphIDStart = 0;
        UnicodeListName = "NULL";
        UnicodeList = [];
        GlyphIDOffsetListName = "NULL";
        GlyphIDOffsetList = [];
        ListLength = 0;
        Type = LVGL_CMAP_TYPE.LV_FONT_FMT_TXT_CMAP_FORMAT0_TINY;
    }

    public int RangeStart { get; set; }
    public int RangeLength { get; set; }
    public int GlyphIDStart { get; set; }
    public string UnicodeListName { get; set; }
    public List<uint> UnicodeList { get; set; }
    public string GlyphIDOffsetListName { get; set; }
    public List<int> GlyphIDOffsetList { get; set; }
    public int ListLength { get; set; }
    public LVGL_CMAP_TYPE Type { get; set; }
}
