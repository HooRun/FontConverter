using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LVGLFontConverter.Library.Models;

public class FontLocaTable
{
    public FontLocaTable()
    {
        GlyphOffsets = new();
    }

    public List<uint> GlyphOffsets { get; set; }
}
