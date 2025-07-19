using static FontConverter.SharedLibrary.Helpers.LVGLFontEnums;

namespace FontConverter.SharedLibrary.Models;

public class LVGLCMapRange
{
    public LVGLCMapRange()
    {
        RangeStart = 0;
        RangeLength = 0;
        BlockName = string.Empty;
        GlyphIDStart = 0;
        UnicodeListName = "NULL";
        UnicodeList = [];
        GlyphIDOffsetListName = "NULL";
        GlyphIDOffsetList = [];
        ListLength = 0;
        Type = LVGL_CMAP_TYPE.LV_FONT_FMT_TXT_CMAP_FORMAT0_TINY;
        Block = new();
        Offset = 0;
    }

    public int RangeStart { get; set; }
    public int RangeLength { get; set; }
    public string BlockName { get; set; }
    public int GlyphIDStart { get; set; }
    public string UnicodeListName { get; set; }
    public List<ushort> UnicodeList { get; set; }
    public string GlyphIDOffsetListName { get; set; }
    public List<ushort> GlyphIDOffsetList { get; set; }
    public int ListLength { get; set; }
    public LVGL_CMAP_TYPE Type { get; set; }
    public UnicodeBlock Block { get; set; }
    public int Offset { get; set; }
}
