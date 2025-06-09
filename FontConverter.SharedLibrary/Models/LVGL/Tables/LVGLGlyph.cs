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
        Index = -1;
        Name = string.Empty;
        Description = string.Empty;
        Bitmap = [];
        Descriptor = new();
        Adjusments = new();
        CodePoints = new();
        Blocks = new();
        IsEmpty = false;
        IsUnMapped = false;
        IsSingleMapped = false;
        IsMultiMapped = false;
    }

    public int Index { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public byte[] Bitmap { get; set; }
    public LVGLGlyphDescriptor Descriptor { get; set; }
    public LVGLFontAdjusments Adjusments { get; set; }
    public SortedList<uint, UnicodeCharacter> CodePoints { get; set; }
    public SortedList<(uint Start, uint End), UnicodeBlock> Blocks { get; set; }

    public bool IsEmpty { get; set; }
    public bool IsUnMapped { get; set; }
    public bool IsSingleMapped { get; set; }
    public bool IsMultiMapped { get; set; }
}
