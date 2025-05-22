namespace FontConverter.SharedLibrary.Models;

public class LVGLFontContents
{
    public LVGLFontContents()
    {
        
    }

    public uint EmptyGlyphsCount { get; set; } = 0;
    public uint UnMappedGlyphsCount { get; set; } = 0;
    public uint SingleMappedGlyphsCount { get; set; } = 0;
    public uint MultiMappedGlyphsCount { get; set; } = 0;

    public List<(uint CodePoint, string Name)> Unicodes { get; set; } = new();
    public SortedDictionary<UnicodeBlock, int> UnicodeRanges { get; set; } = new();
}
