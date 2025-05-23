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
        IsEmpty = false;
        IsUnMapped = false;
        IsSingleMapped = false;
        IsMultiMapped = false;
        CodePoints = new SortedList<int, UnicodeCharacter>();
    }

    public string Name { get; set; }
    public string Description { get; set; }
    public int Index { get; set; }
    public byte[] Bitmap { get; set; }
    public LVGLGlyphDescriptor Descriptor { get; set; }

    public bool IsEmpty { get; set; }
    public bool IsUnMapped { get; set; }
    public bool IsSingleMapped { get; set; }
    public bool IsMultiMapped { get; set; }

    public SortedList<int, UnicodeCharacter> CodePoints { get; set; }

}
