namespace FontConverter.SharedLibrary.Models;

public class LVGLFontContents : LVGLFontContent
{
    public LVGLFontContents()
    {
        GlyphsCount = 0;
        EmptyGlyphsCount = 0;
        UnMappedGlyphsCount = 0;
        SingleMappedGlyphsCount = 0;
        MultiMappedGlyphsCount = 0;
        UnicodesCount = 0;

        Contents.Add(GlyphsHeader, new LVGLFontContent(GlyphsHeader, GlyphsIcon, GlyphsCount, true, new SortedList<string, LVGLFontContent>()));
        Contents[GlyphsHeader].Contents.Add(EmptyGlyphsHeader, new LVGLFontContent(EmptyGlyphsHeader, EmptyGlyphsIcon, EmptyGlyphsCount, false, new SortedList<string, LVGLFontContent>()));
        Contents[GlyphsHeader].Contents.Add(UnMappedGlyphsHeader, new LVGLFontContent(UnMappedGlyphsHeader, UnMappedGlyphsIcon, UnMappedGlyphsCount, false, new SortedList<string, LVGLFontContent>()));
        Contents[GlyphsHeader].Contents.Add(SingleMappedGlyphsHeader, new LVGLFontContent(SingleMappedGlyphsHeader, SingleMappedGlyphsIcon, SingleMappedGlyphsCount, false, new SortedList<string, LVGLFontContent>()));
        Contents[GlyphsHeader].Contents.Add(MultiMappedGlyphsHeader, new LVGLFontContent(MultiMappedGlyphsHeader, MultiMappedGlyphsIcon, MultiMappedGlyphsCount, false, new SortedList<string, LVGLFontContent>()));
        Contents.Add(UnicodesHeader, new LVGLFontContent(UnicodesHeader, UnicodesIcon, UnicodesCount, false, new SortedList<string, LVGLFontContent>()));
    }

    
    public int GlyphsCount { get; set; }
    public int EmptyGlyphsCount { get; set; }
    public int UnMappedGlyphsCount { get; set; }
    public int SingleMappedGlyphsCount { get; set; }
    public int MultiMappedGlyphsCount { get; set; }
    public int UnicodesCount { get; set; }

    public const string GlyphsHeader  = "Glyphs";
    public const string EmptyGlyphsHeader = "Empty Glyphs";
    public const string UnMappedGlyphsHeader = "Unmapped Glyphs";
    public const string SingleMappedGlyphsHeader = "Single Mapped Glyphs";
    public const string MultiMappedGlyphsHeader = "Multi Mapped Glyphs";
    public const string UnicodesHeader = "Unicodes";

    public const string GlyphsIcon = "glyphs";
    public const string EmptyGlyphsIcon = "language_chinese_dayi";
    public const string UnMappedGlyphsIcon = "language_chinese_dayi";
    public const string SingleMappedGlyphsIcon = "language_chinese_dayi";
    public const string MultiMappedGlyphsIcon = "language_chinese_dayi";
    public const string UnicodesIcon = "language";
    public const string UnicodeRangeIcon = "text_fields";
}
