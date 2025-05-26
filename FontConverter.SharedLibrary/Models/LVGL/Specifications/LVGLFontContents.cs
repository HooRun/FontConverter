namespace FontConverter.SharedLibrary.Models;

public class LVGLFontContents : LVGLFontContent
{
    public LVGLFontContents()
    {
        Contents.Add(GlyphsHeader, new LVGLFontContent(GlyphsHeader, GlyphsIcon, 0, true, new SortedList<string, LVGLFontContent>()));
        Contents[GlyphsHeader].Contents.Add(EmptyGlyphsHeader, new LVGLFontContent(EmptyGlyphsHeader, EmptyGlyphsIcon, 0, false, new SortedList<string, LVGLFontContent>()));
        Contents[GlyphsHeader].Contents.Add(UnMappedGlyphsHeader, new LVGLFontContent(UnMappedGlyphsHeader, UnMappedGlyphsIcon, 0, false, new SortedList<string, LVGLFontContent>()));
        Contents[GlyphsHeader].Contents.Add(SingleMappedGlyphsHeader, new LVGLFontContent(SingleMappedGlyphsHeader, SingleMappedGlyphsIcon, 0, false, new SortedList<string, LVGLFontContent>()));
        Contents[GlyphsHeader].Contents.Add(MultiMappedGlyphsHeader, new LVGLFontContent(MultiMappedGlyphsHeader, MultiMappedGlyphsIcon, 0, false, new SortedList<string, LVGLFontContent>()));
        Contents.Add(UnicodesHeader, new LVGLFontContent(UnicodesHeader, UnicodesIcon, 0, false, new SortedList<string, LVGLFontContent>()));
    }

    public string GlyphsHeader { get; }  = "Glyphs";
    public string EmptyGlyphsHeader { get; } = "Empty Glyphs";
    public string UnMappedGlyphsHeader { get; } = "Unmapped Glyphs";
    public string SingleMappedGlyphsHeader { get; } = "Single Mapped Glyphs";
    public string MultiMappedGlyphsHeader { get; } = "Multi Mapped Glyphs";
    public string UnicodesHeader { get; } = "Unicodes";

    public string GlyphsIcon { get; } = "glyphs";
    public string EmptyGlyphsIcon { get; } = "language_chinese_dayi";
    public string UnMappedGlyphsIcon { get; } = "language_chinese_dayi";
    public string SingleMappedGlyphsIcon { get; } = "language_chinese_dayi";
    public string MultiMappedGlyphsIcon { get; } = "language_chinese_dayi";
    public string UnicodesIcon { get; } = "language";
    public string UnicodeRangeIcon { get; } = "text_fields";
}
