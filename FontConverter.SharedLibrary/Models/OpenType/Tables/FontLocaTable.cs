namespace FontConverter.SharedLibrary.Models;

public class FontLocaTable
{
    public FontLocaTable()
    {
        GlyphOffsets = new();
    }

    public List<uint> GlyphOffsets { get; set; }
}
