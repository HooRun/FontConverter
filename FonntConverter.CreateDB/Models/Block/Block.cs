using MessagePack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FonntConverter.CreateDB.Models;

[DebuggerDisplay("{DebuggerView}")]
[MessagePackObject]
public class Block
{
    public Block()
    {
        Start = 0;
        End = 0;
        Name = string.Empty;
        Characters = new();
    }

    public Block(uint start, uint end, string name) : this()
    {
        Start = start;
        End = end;
        Name = name ?? string.Empty;
    }

    [Key(0)]
    public uint Start { get; set; }
    [Key(1)]
    public uint End { get; set; }
    [Key(2)]
    public string Name { get; set; }
    [Key(3)]
    public SortedDictionary<uint, Character> Characters { get; set; }


    private string DebuggerView => $"U+{Start:X6}-U+{End:X6} {Name}";

    public uint? GetBlockStart(uint codepoint)
    {
        if (Start <= codepoint && codepoint <= End)
            return Start;
        return null;
    }

    public Character? GetCharacter(uint codePoint)
    {
        if (Characters.ContainsKey(codePoint))
            return Characters[codePoint];
        return null;
    }
}
