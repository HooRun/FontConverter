using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FontConverter.SharedLibrary.Models;

public class UnicodeBlockCollection
{
    public UnicodeBlockCollection()
    {
        Blocks = new();
        AllCharacters = new();
        AllBlocks = new();
    }

    public SortedList<(uint Start, uint End), UnicodeBlock> Blocks { get; set; }
    public Dictionary<uint, UnicodeCharacter> AllCharacters { get; set; }
    public Dictionary<(uint Start, uint End), UnicodeBlock> AllBlocks { get; set; }
}
