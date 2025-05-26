namespace FontConverter.SharedLibrary.Models;

public class FontCmapTable
{
    public FontCmapTable()
    {
        UnicodeToGlyphMap = new();
        GlyphToUnicodeMap = new();
    }

    public SortedDictionary<uint, ushort> UnicodeToGlyphMap { get; set; }
    public SortedDictionary<ushort, List<uint>> GlyphToUnicodeMap { get; set; }
}
