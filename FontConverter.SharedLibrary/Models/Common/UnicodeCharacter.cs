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
        Name = string.Empty;
    }

    public UnicodeCharacter(uint codePoint, string name) : this()
    {
        CodePoint = codePoint;
        Name = name ?? string.Empty;
    }

    public uint CodePoint { get; set; }
    public string Name { get; set; }
}
