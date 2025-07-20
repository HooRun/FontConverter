using System.Collections.Generic;

namespace LVGLFontConverter.Library.Models;

public class FontCmapTable
{
    public FontCmapTable()
    {
        UnicodeToGlyphMap = new();
    }

    public SortedDictionary<uint, ushort> UnicodeToGlyphMap { get; set; }
}
