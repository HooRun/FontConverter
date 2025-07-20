namespace FontConverter.SharedLibrary.Models;

public class FontMaxpTable
{
    public FontMaxpTable()
    {
        
    }

    public double Version { get; set; } // 0.5 or 1.0
    public ushort NumGlyphs { get; set; }

    // Fields present only in version 1.0
    public ushort MaxPoints { get; set; }
    public ushort MaxContours { get; set; }
    public ushort MaxCompositePoints { get; set; }
    public ushort MaxCompositeContours { get; set; }
    public ushort MaxZones { get; set; }
    public ushort MaxTwilightPoints { get; set; }
    public ushort MaxStorage { get; set; }
    public ushort MaxFunctionDefs { get; set; }
    public ushort MaxInstructionDefs { get; set; }
    public ushort MaxStackElements { get; set; }
    public ushort MaxSizeOfInstructions { get; set; }
    public ushort MaxComponentElements { get; set; }
    public ushort MaxComponentDepth { get; set; }
}
