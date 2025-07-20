using MessagePack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FonntConverter.CreateDB.Helpers.UCDEnumsHelper;

namespace FonntConverter.CreateDB.Models;

[DebuggerDisplay("{DebuggerView}")]
[MessagePackObject]
public class Character
{
    public Character()
    {
        CodePoint = 0;
        Name = string.Empty;
        DecompositionType = DecompositionTypeEnum.DECOMPOSITION_TYPE_NONE;
        DecompositionMapping = [];
        Block = 0;
    }

    public Character(uint codePoint, string? name) : this()
    {
        CodePoint = codePoint;
        Name = name ?? string.Empty;
    }

    public Character(uint codePoint, string? name, uint block) : this()
    {
        CodePoint = codePoint;
        Name = name ?? string.Empty;
        Block = block;
    }

    public Character(uint codePoint, string? name, DecompositionTypeEnum decompositionType, List<uint> decompositionMapping) : this()
    {
        CodePoint = codePoint;
        Name = name ?? string.Empty;
        DecompositionType = decompositionType;
        DecompositionMapping = decompositionMapping ?? [];
    }


    [Key(0)]
    public uint CodePoint { get; set; }
    [Key(1)]
    public string Name { get; set; }
    [Key(2)]
    public DecompositionTypeEnum DecompositionType { get; set; }
    [Key(3)]
    public List<uint> DecompositionMapping { get; set; }
    [Key(4)]
    public uint Block {  get; set; }

    private string DebuggerView => $"U+{CodePoint:X6} {Name}";
}

