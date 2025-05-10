using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LVGLFontConverter.Library.Helpers.LVGLFontEnums;

namespace LVGLFontConverter.Models;

public class FontBitPerPixel
{
    public FontBitPerPixel()
    {
        
    }

    public BIT_PER_PIXEL_ENUM BPP { get; set; }
    public string Description { get; set; } = string.Empty;
}
