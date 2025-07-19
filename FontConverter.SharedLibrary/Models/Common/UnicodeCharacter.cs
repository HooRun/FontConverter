using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FontConverter.SharedLibrary.Helpers.UCDEnumsHelper;

namespace FontConverter.SharedLibrary.Models;

[MessagePackObject]
public class UnicodeCharacter
{
    public UnicodeCharacter()
    {
        CodePoint = 0;
        Name = string.Empty;
        DecompositionType = DecompositionTypeEnum.DECOMPOSITION_TYPE_NONE;
        DecompositionMapping = [];
        GlyphID = null;
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
    public uint Block { get; set; }
    [IgnoreMember]
    public int? GlyphID { get; set; }
    [IgnoreMember]
    public string CodePointString => $"U+{CodePoint:X6}";
}
