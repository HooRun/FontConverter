using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FontConverter.SharedLibrary.Models;

public class LVGLGlyph
{
    public LVGLGlyph()
    {
        Name = string.Empty;
        Description = string.Empty;
        Index = 0;
        Bitmap = [];
        Descriptor = new();
    }

    public string Name { get; set; }
    public string Description { get; set; }
    public int Index { get; set; }
    public byte[] Bitmap { get; set; }
    public LVGLGlyphDescriptor Descriptor { get; set; }
}
