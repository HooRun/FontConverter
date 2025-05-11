using LVGLFontConverter.Library.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LVGLFontConverter.Library.Models.LVGL.FontDetails;

public class LVGLFontContent
{
    public LVGLFontContent()
    {
        
    }

    public uint EmptyGlyphsCount { get; set; } = 0;
    public uint UnMappedGlyphsCount { get; set; } = 0;
    public uint SingleMappedGlyphsCount { get; set; } = 0;
    public uint MultiMappedGlyphsCount { get; set; } = 0;

    public List<(uint CodePoint, string Name)> Unicodes { get; set; } = new();
    public SortedDictionary<UnicodeBlock, int> UnicodeRanges { get; set; } = new();
}
