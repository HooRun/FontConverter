using LVGLFontConverter.Library.Models;
using static LVGLFontConverter.Library.Helpers.FontTablesEnum;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace LVGLFontConverter.Library.Models.OpenType;

public class OpenTypeFont
{
    public OpenTypeFont()
    {
        Tables = new();
        NameTable = new();
        HeadTable = new();
        HheaTable = new();
        VheaTable = new();
        OS2Table = new();
        MaxpTable = new();
        PostTable = new();
        CmapTable = new();
        LocaTable = new();
        GlyfTable = new();
        HmtxTable = new();
        VmtxTable = new();
        KernTable = new();
    }

    public SortedDictionary<OpenTypeTables, OpenTypeTable> Tables { get; set; } 

    public FontNameTable NameTable { get; set; }

    public FontOS2Table OS2Table { get; set; }

    public FontPostTable PostTable { get; set; }

    public FontHeadTable HeadTable { get; set; }

    public FontHheaTable HheaTable { get; set; }

    public FontVheaTable VheaTable { get; set; }

    public FontCmapTable CmapTable { get; set; }

    public FontMaxpTable MaxpTable { get; set; }

    public FontHmtxTable HmtxTable { get; set; }

    public FontVmtxTable VmtxTable { get; set; }

    public FontKernTable KernTable { get; set; }

    public FontLocaTable LocaTable { get; set; }

    public FontGlyfTable GlyfTable { get; set; }

}
