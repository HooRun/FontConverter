using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FontConverter.SharedLibrary.Models;

public class UnicodeBlock
{
    public UnicodeBlock()
    {
        Start = 0;
        End = 0;
        Name = string.Empty;
        Characters = new();
    }

    public UnicodeBlock(uint start, uint end, string name) : this()
    {
        Start = start;
        End = end;
        Name = name ?? string.Empty;
    }

    public uint Start { get; set; }
    public uint End { get; set; }
    public string Name { get; set; }

    public SortedList<uint, UnicodeCharacter> Characters { get; set; }
}
