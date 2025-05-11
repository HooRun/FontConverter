using LVGLFontConverter.Library.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LVGLFontConverter.Library.Models;

public class LVGLFontGlyph
{
    public LVGLFontGlyph()
    {
        Unicodes = [];

        IsEmpty = false;
        IsUnMapped = false;
        IsMultiMapped = false;

    }

    public int Index { get; set; }
    
    public LVGLGlyphProperties GlyphProperties { get; set; } = new();
    public LVGLFontAdjusment GlyphAdjusment { get; set; } = new();
    public LVGLGlyphBitmap GlyphBitmap { get; set; } = new();

    

    

    
    
    public List<(uint CodePoint, string Name)> Unicodes { get; set; }

    public bool IsEmpty { get; set; }
    public bool IsUnMapped { get; set; }
    public bool IsSingleMapped { get; set; }
    public bool IsMultiMapped { get; set; }

}
