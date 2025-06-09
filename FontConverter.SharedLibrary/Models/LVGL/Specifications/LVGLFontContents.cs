namespace FontConverter.SharedLibrary.Models;

public class LVGLFontContents : LVGLFontContent
{
    public LVGLFontContents()
    {
        Contents.Add(GlyphsHeader, new LVGLFontContent(GlyphsHeader, GlyphsSubTitle, GlyphsIcon, 0, true, null, new SortedList<string, LVGLFontContent>()));
        Contents[GlyphsHeader].Contents.Add(EmptyGlyphsHeader, new LVGLFontContent(EmptyGlyphsHeader, EmptyGlyphsSubTitle, EmptyGlyphsIcon, 0, false, null, new SortedList<string, LVGLFontContent>()));
        Contents[GlyphsHeader].Contents.Add(UnMappedGlyphsHeader, new LVGLFontContent(UnMappedGlyphsHeader, UnMappedGlyphsSubTitle, UnMappedGlyphsIcon, 0, false, null, new SortedList<string, LVGLFontContent>()));
        Contents[GlyphsHeader].Contents.Add(SingleMappedGlyphsHeader, new LVGLFontContent(SingleMappedGlyphsHeader, SingleMappedGlyphsSubTitle, SingleMappedGlyphsIcon, 0, false, null, new SortedList<string, LVGLFontContent>()));
        Contents[GlyphsHeader].Contents.Add(MultiMappedGlyphsHeader, new LVGLFontContent(MultiMappedGlyphsHeader, MultiMappedGlyphsSubTitle, MultiMappedGlyphsIcon, 0, false, null, new SortedList<string, LVGLFontContent>()));
        Contents.Add(UnicodesHeader, new LVGLFontContent(UnicodesHeader, UnicodesSubTitle, UnicodesIcon, 0, false, null, new SortedList<string, LVGLFontContent>()));
    }

    public string GlyphsHeader { get; }  = "Glyphs";
    public string GlyphsSubTitle { get; } = "";

    public string EmptyGlyphsHeader { get; } = "Empty Glyphs";
    public string EmptyGlyphsSubTitle { get; } = "";

    public string UnMappedGlyphsHeader { get; } = "Unmapped Glyphs";
    public string UnMappedGlyphsSubTitle { get; } = "";

    public string SingleMappedGlyphsHeader { get; } = "Single Mapped Glyphs";
    public string SingleMappedGlyphsSubTitle { get; } = "";

    public string MultiMappedGlyphsHeader { get; } = "Multi Mapped Glyphs";
    public string MultiMappedGlyphsSubTitle { get; } = "";

    public string UnicodesHeader { get; } = "Unicodes";
    public string UnicodesSubTitle { get; } = "";

    public string GlyphsIcon { get; } = "glyphs";
    public string EmptyGlyphsIcon { get; } = "language_chinese_dayi";
    public string UnMappedGlyphsIcon { get; } = "language_chinese_dayi";
    public string SingleMappedGlyphsIcon { get; } = "language_chinese_dayi";
    public string MultiMappedGlyphsIcon { get; } = "language_chinese_dayi";
    public string UnicodesIcon { get; } = "language";
    public string UnicodeRangeIcon { get; } = "text_fields";
}
