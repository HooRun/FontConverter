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
        Start = -1;
        End = -1;
        Name = string.Empty;
        ContainsAnyCharacterInFont = false;
        Characters = new();
    }

    public UnicodeBlock(int start, int end, string name) : this()
    {
        Start = start;
        End = end;
        Name = name ?? string.Empty;
    }

    public int Start { get; set; }
    public int End { get; set; }
    public string Name { get; set; }
    public bool ContainsAnyCharacterInFont { get; set; }

    public SortedList<int, UnicodeCharacter> Characters { get; set; }
}
