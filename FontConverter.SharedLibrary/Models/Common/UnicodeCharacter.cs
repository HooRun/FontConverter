using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FontConverter.SharedLibrary.Models;

public class UnicodeCharacter
{
    public UnicodeCharacter()
    {
        CodePoint = 0;
        CodePointString = string.Empty;
        Name = string.Empty;
        GlyphID = null;
        ParentBlock = null;
    }

    public UnicodeCharacter(uint codePoint, string name, UnicodeBlock? parent) : this()
    {
        CodePoint = codePoint;
        CodePointString = $"U+{CodePoint:X6}";
        Name = name ?? string.Empty;
        ParentBlock = parent;
    }

    public uint CodePoint { get; set; }
    public string CodePointString { get; set; }
    public string Name { get; set; }
    public int? GlyphID { get; set; }
    public UnicodeBlock? ParentBlock { get; set; }
}
