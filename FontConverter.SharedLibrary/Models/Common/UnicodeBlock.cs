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
        StartString = string.Empty;
        End = 0;
        EndString = string.Empty;
        Name = string.Empty;
        Characters = new();
    }

    public UnicodeBlock(uint start, uint end, string name) : this()
    {
        Start = start;
        StartString = $"U+{Start:X6}";
        End = end;
        EndString = $"U+{End:X6}";
        Name = name ?? string.Empty;
    }

    public UnicodeBlock(UnicodeBlock block) : this()
    {
        Start = block.Start;
        End = block.End;
        Name = block.Name ?? string.Empty;
    }

    public uint Start { get; set; }
    public string StartString { get; set; }
    public uint End { get; set; }
    public string EndString { get; set; }
    public string Name { get; set; }

    public SortedList<uint, UnicodeCharacter> Characters { get; set; }
}
