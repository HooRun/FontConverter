using SkiaSharp;
using static FontConverter.SharedLibrary.Helpers.FontTablesEnumHelper;

namespace FontConverter.SharedLibrary.Models;
public class OpenTypeFont
{
    public OpenTypeFont()
    {
        SKTypeface = null;
        SKFont = null;
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
        IsValid = false;
    }

    public void UpdateTables(OpenTypeFont openTypeFont) 
    {
        NameTable = openTypeFont.NameTable;
        OS2Table = openTypeFont.OS2Table;
        PostTable = openTypeFont.PostTable;
        HeadTable = openTypeFont.HeadTable;
        HheaTable = openTypeFont.HheaTable;
        VheaTable = openTypeFont.VheaTable;
        CmapTable = openTypeFont.CmapTable;
        MaxpTable = openTypeFont.MaxpTable;
        HmtxTable = openTypeFont.HmtxTable;
        VmtxTable = openTypeFont.VmtxTable;
        KernTable = openTypeFont.KernTable;
        LocaTable = openTypeFont.LocaTable;
        GlyfTable = openTypeFont.GlyfTable;
    }

    public SKTypeface? SKTypeface { get; set; }
    public SKFont? SKFont { get; set; }

    public SortedList<OpenTypeTables, OpenTypeTableBinaryData> Tables { get; set; } 

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

    public bool IsValid { get; set; }

}
