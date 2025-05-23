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
        CodePoint = -1;
        Name = string.Empty;
    }

    public UnicodeCharacter(int codePoint, string name) : this()
    {
        CodePoint = codePoint;
        Name = name ?? string.Empty;
    }

    public int CodePoint { get; set; }
    public string Name { get; set; }
}
