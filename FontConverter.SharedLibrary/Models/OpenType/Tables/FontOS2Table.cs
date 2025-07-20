using static FontConverter.SharedLibrary.Helpers.FontTablesEnumHelper;

namespace FontConverter.SharedLibrary.Models;

public class FontOS2Table
{
    public FontOS2Table()
    {
        Panose = new();
        AchVendID = string.Empty;

        FsType = new();
        UnicodeRange1 = new();
        UnicodeRange2 = new();
        UnicodeRange3 = new();
        UnicodeRange4 = new();
        CodePageRange1 = new();
        CodePageRange2 = new();
    }

    // OS/2 Table Information
    public ushort Version { get; set; }
    public short XAvgCharWidth { get; set; }
    public ushort UsWeightClass { get; set; }
    public ushort UsWidthClass { get; set; }
    public List<FsTypeFlags> FsType { get; set; }
    public short YSubscriptXSize { get; set; }
    public short YSubscriptYSize { get; set; }
    public short YSubscriptXOffset { get; set; }
    public short YSubscriptYOffset { get; set; }
    public short YSuperscriptXSize { get; set; }
    public short YSuperscriptYSize { get; set; }
    public short YSuperscriptXOffset { get; set; }
    public short YSuperscriptYOffset { get; set; }
    public short YStrikeoutSize { get; set; }
    public short YStrikeoutPosition { get; set; }
    public short SFamilyClass { get; set; }
    public FontPANOSERecord Panose { get; set; }
    public List<UnicodeRange1> UnicodeRange1 { get; set; }
    public List<UnicodeRange2> UnicodeRange2 { get; set; }
    public List<UnicodeRange3> UnicodeRange3 { get; set; }
    public List<UnicodeRange4> UnicodeRange4 { get; set; }
    public string AchVendID { get; set; }
    public ushort FsSelection { get; set; }
    public ushort UsFirstCharIndex { get; set; }
    public ushort UsLastCharIndex { get; set; }
    public short STypoAscender { get; set; }
    public short STypoDescender { get; set; }
    public short STypoLineGap { get; set; }
    public ushort UsWinAscent { get; set; }
    public ushort UsWinDescent { get; set; }
    public List<CodePageRange1> CodePageRange1 { get; set; }
    public List<CodePageRange2> CodePageRange2 { get; set; }
    public short SxHeight { get; set; }
    public short SCapHeight { get; set; }
    public ushort UsDefaultChar { get; set; }
    public ushort UsBreakChar { get; set; }
    public ushort UsMaxContext { get; set; }
    public ushort UsLowerOpticalPointSize { get; set; }
    public ushort UsUpperOpticalPointSize { get; set; }
}

public class FontPANOSERecord
{
    public FontPANOSERecord()
    {

    }

    public byte FamilyType { get; set; }
    public byte SerifStyle { get; set; }
    public byte Weight { get; set; }
    public byte Proportion { get; set; }
    public byte Contrast { get; set; }
    public byte StrokeVariation { get; set; }
    public byte ArmStyle { get; set; }
    public byte LetterForm { get; set; }
    public byte Midline { get; set; }
    public byte XHeight { get; set; }
}

