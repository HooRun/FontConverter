using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FontConverter.SharedLibrary.Models;

[MessagePackObject]
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

    public UnicodeBlock(UnicodeBlock block) : this()
    {
        Start = block.Start;
        End = block.End;
        Name = block.Name ?? string.Empty;
    }

    [Key(0)]
    public uint Start { get; set; }
    [Key(1)]
    public uint End { get; set; }
    [Key(2)]
    public string Name { get; set; }
    [Key(3)]
    public SortedDictionary<uint, UnicodeCharacter> Characters { get; set; }
    [IgnoreMember]
    public string StartString => $"U+{Start:X6}";
    [IgnoreMember]
    public string EndString => $"U+{End:X6}";
    [IgnoreMember]
    public uint Length => (End - Start + 1);

}
