using System;

namespace LVGLFontConverter.Library.Helpers;

public static class FontTablesEnum
{
    public enum OpenTypeTables : uint
    {
        /// <summary>
        /// Variation Axis Values Table.
        /// </summary>
        AVAR = 0x61766172, // 'avar'

        /// <summary>
        /// Baseline Table.
        /// </summary>
        BASE = 0x42415345, // 'BASE'

        /// <summary>
        /// Color Bitmap Data Table.
        /// </summary>
        CBDT = 0x43424454, // 'CBDT'

        /// <summary>
        /// Color Bitmap Location Table.
        /// </summary>
        CBLC = 0x43424C43, // 'CBLC'

        /// <summary>
        /// Compact _font Format Table.
        /// </summary>
        CFF = 0x43464620, // 'CFF '

        /// <summary>
        /// Compact _font Format 2 Table.
        /// </summary>
        CFF2 = 0x43464632, // 'CFF2'

        /// <summary>
        /// Character to Glyph Mapping Table.
        /// </summary>
        CMAP = 0x636D6170, // 'cmap'

        /// <summary>
        /// Color Table.
        /// </summary>
        COLR = 0x434F4C52, // 'COLR'

        /// <summary>
        /// Color Palette Table.
        /// </summary>
        CPAL = 0x4350414C, // 'CPAL'

        /// <summary>
        /// Character Variation Table.
        /// </summary>
        CVAR = 0x63766172, // 'cvar'

        /// <summary>
        /// Control Value Table.
        /// </summary>
        CVT = 0x63767420, // 'cvt '

        /// <summary>
        /// Digital Signature Table.
        /// </summary>
        DSIG = 0x44534947, // 'DSIG'

        /// <summary>
        /// Embedded Bitmap Data Table.
        /// </summary>
        EBDT = 0x45424454, // 'EBDT'

        /// <summary>
        /// Embedded Bitmap Location Table.
        /// </summary>
        EBLC = 0x45424C43, // 'EBLC'

        /// <summary>
        /// Embedded Scalable Bitmap Table.
        /// </summary>
        EBSC = 0x45425343, // 'EBSC'

        /// <summary>
        /// _font Program Table.
        /// </summary>
        FPGM = 0x6670676D, // 'fpgm'

        /// <summary>
        /// _font Variations Table.
        /// </summary>
        FVAR = 0x66766172, // 'fvar'

        /// <summary>
        /// Grid-fitting and Scan Conversion Procedure Table.
        /// </summary>
        GASP = 0x67617370, // 'gasp'

        /// <summary>
        /// Glyph Definition Table.
        /// </summary>
        GDEF = 0x47444546, // 'GDEF'

        /// <summary>
        /// Glyph Data Table.
        /// </summary>
        GLYF = 0x676C7966, // 'glyf'

        /// <summary>
        /// Glyph Positioning Table.
        /// </summary>
        GPOS = 0x47504F53, // 'GPOS'

        /// <summary>
        /// Glyph Substitution Table.
        /// </summary>
        GSUB = 0x47535542, // 'GSUB'

        /// <summary>
        /// Glyph Variations Table.
        /// </summary>
        GVAR = 0x67766172, // 'gvar'

        /// <summary>
        /// Horizontal Device Metrics Table.
        /// </summary>
        HDMX = 0x68646D78, // 'hdmx'

        /// <summary>
        /// _font Header Table.
        /// </summary>
        HEAD = 0x68656164, // 'head'

        /// <summary>
        /// Horizontal Header Table.
        /// </summary>
        HHEA = 0x68686561, // 'hhea'

        /// <summary>
        /// Horizontal Metrics Table.
        /// </summary>
        HMTX = 0x686D7478, // 'hmtx'

        /// <summary>
        /// Horizontal Variation Table.
        /// </summary>
        HVAR = 0x48564152, // 'HVAR'

        /// <summary>
        /// Justification Table.
        /// </summary>
        JSTF = 0x4A535446, // 'JSTF'

        /// <summary>
        /// Kerning Table.
        /// </summary>
        KERN = 0x6B65726E, // 'kern'

        /// <summary>
        /// Index to Location Table.
        /// </summary>
        LOCA = 0x6C6F6361, // 'loca'

        /// <summary>
        /// Linear Threshold Table.
        /// </summary>
        LTSH = 0x4C545348, // 'LTSH'

        /// <summary>
        /// Mathematical Layout Table.
        /// </summary>
        MATH = 0x4D415448, // 'MATH'

        /// <summary>
        /// Maximum Profile Table.
        /// </summary>
        MAXP = 0x6D617870, // 'maxp'

        /// <summary>
        /// Glyph Merging Table.
        /// </summary>
        MERG = 0x4D455247, // 'MERG'

        /// <summary>
        /// Metadata Table.
        /// </summary>
        META = 0x6D657461, // 'meta'

        /// <summary>
        /// Metric Variations Table.
        /// </summary>
        MVAR = 0x4D564152, // 'MVAR'

        /// <summary>
        /// Naming Table.
        /// </summary>
        NAME = 0x6E616D65, // 'name'

        /// <summary>
        /// OS/2 and Windows Specific Metrics Table.
        /// </summary>
        OS2 = 0x4F532F32, // 'OS/2'

        /// <summary>
        /// PCL 5 Table.
        /// </summary>
        PCLT = 0x50434C54, // 'PCLT'

        /// <summary>
        /// PostScript Table.
        /// </summary>
        POST = 0x706F7374, // 'post'

        /// <summary>
        /// Pre-program Table.
        /// </summary>
        PREP = 0x70726570, // 'prep'

        /// <summary>
        /// Standard Bitmap Graphics Table.
        /// </summary>
        SBIX = 0x73626978, // 'sbix'

        /// <summary>
        /// Style Attributes Table.
        /// </summary>
        STAT = 0x53544154, // 'STAT'

        /// <summary>
        /// Scalable Vector Graphics Table.
        /// </summary>
        SVG = 0x53564720, // 'SVG '

        /// <summary>
        /// Vertical Device Metrics Table.
        /// </summary>
        VDMX = 0x56444D58, // 'VDMX'

        /// <summary>
        /// Vertical Header Table.
        /// </summary>
        VHEA = 0x76686561, // 'vhea'

        /// <summary>
        /// Vertical Metrics Table.
        /// </summary>
        VMTX = 0x766D7478, // 'vmtx'

        /// <summary>
        /// Vertical Origin Table.
        /// </summary>
        VORG = 0x564F5247, // 'VORG'

        /// <summary>
        /// Vertical Variation Table.
        /// </summary>
        VVAR = 0x56564152  // 'VVAR'
    }

    public enum NameType : ushort
    {
        Copyright = 0,
        FontFamily = 1,
        FontSubfamily = 2,
        UniqueIdentifier = 3,
        FullFontName = 4,
        Version = 5,
        PostScriptName = 6,
        Trademark = 7,
        Manufacturer = 8,
        Designer = 9,
        Description = 10,
        VendorURL = 11,
        DesignerURL = 12,
        License = 13,
        LicenseURL = 14,
        PreferredFamily = 16,
        PreferredSubfamily = 17,
        CompatibleFull = 18,
        SampleText = 19,
        PostScriptCID = 20,
        WWSFamily = 21,
        WWSSubfamily = 22,
        LightBackgroundPalette = 23,
        DarkBackgroundPalette = 24,
        VariationsPostScriptPrefix = 25
    }

    [Flags]
    public enum HeadFlags : ushort
    {
        BASELINE_AT_Y_0 = 1 << 0, // Bit 0: Baseline for font at y=0.
        LEFT_SIDEBEARING_AT_X_0 = 1 << 1, // Bit 1: Left sidebearing point at x=0.
        INSTRUCTIONS_DEPEND_ON_SIZE = 1 << 2, // Bit 2: Instructions may depend on point size.
        FORCE_INTEGER_PPEM = 1 << 3, // Bit 3: Force ppem to integer values.
        INSTRUCTIONS_ALTER_ADVANCE = 1 << 4, // Bit 4: Instructions may alter advance width.
        UNUSED_BIT_5 = 1 << 5, // Bit 5: Unused in OpenType.
        LOSSLESS_DATA = 1 << 11, // Bit 11: _font data is "lossless".
        CONVERTED_METRICS = 1 << 12, // Bit 12: _font converted (produce compatible metrics).
        CLEARTYPE_OPTIMIZED = 1 << 13, // Bit 13: _font optimized for ClearType.
        LAST_RESORT_FONT = 1 << 14, // Bit 14: Last Resort font.
    }

    [Flags]
    public enum MacStyleFlags : ushort
    {
        BOLD = 1 << 0,
        ITALIC = 1 << 1,
        UNDERLINE = 1 << 2,
        OUTLINE = 1 << 3,
        SHADOW = 1 << 4,
        CONDENSED = 1 << 5,
        EXTENDED = 1 << 6
    }

    public enum FontDirectionHint : short
    {
        FullyMixed = 0,
        OnlyStrongLTR = 1,
        StrongLTRWithNeutrals = 2,
        ReservedMinus1 = -1,
        ReservedMinus2 = -2
    }

    // FsType embedding permissions
    [Flags]
    public enum FsTypeFlags : ushort
    {
        Installable = 0x0000,
        Restricted = 0x0002,
        PreviewPrint = 0x0004,
        Editable = 0x0008,
        NoSubsetting = 0x0100,
        BitmapEmbeddingOnly = 0x0200
    }

    // FsSelection style bits
    [Flags]
    public enum FsSelectionFlags : ushort
    {
        Italic = 0x0001,
        Underscore = 0x0002,
        Negative = 0x0004,
        Outlined = 0x0008,
        Strikeout = 0x0010,
        Bold = 0x0020,
        Regular = 0x0040,
        UseTypoMetrics = 0x0080,
        WWS = 0x0100,
        Oblique = 0x0200
    }

    // Weight class
    public enum WeightClass : ushort
    {
        Thin = 100,
        ExtraLight = 200,
        Light = 300,
        Normal = 400,
        Medium = 500,
        SemiBold = 600,
        Bold = 700,
        ExtraBold = 800,
        Black = 900
    }

    // Width class
    public enum WidthClass : ushort
    {
        UltraCondensed = 1,
        ExtraCondensed = 2,
        Condensed = 3,
        SemiCondensed = 4,
        Medium = 5,
        SemiExpanded = 6,
        Expanded = 7,
        ExtraExpanded = 8,
        UltraExpanded = 9
    }

    // Unicode Ranges - bitfields in 4 UInt32s
    [Flags]
    public enum UnicodeRange1 : uint
    {
        R0000_007F = 1U << 0, //Basic Latin
        R0080_00FF = 1U << 1, //Latin-1 Supplement
        R0100_017F = 1U << 2, //Latin Extended-A
        R0180_024F = 1U << 3, //Latin Extended-B
        R0250_02AF = 1U << 4, //IPA Extensions
        R1D00_1D7F = 1U << 4, //Phonetic Extensions
        R1D80_1DBF = 1U << 4, //Phonetic Extensions Supplement
        R02B0_02FF = 1U << 5, //Spacing Modifier Letters
        RA700_A71F = 1U << 5, //Modifier Tone Letters
        R0300_036F = 1U << 6, //Combining Diacritical Marks
        R1DC0_1DFF = 1U << 6, //Combining Diacritical Marks Supplement
        R0370_03FF = 1U << 7, //Greek and Coptic
        R2C80_2CFF = 1U << 8, //Coptic
        R0400_04FF = 1U << 9, //Cyrillic
        R0500_052F = 1U << 9, //Cyrillic Supplement
        R2DE0_2DFF = 1U << 9, //Cyrillic Extended-A
        RA640_A69F = 1U << 9, //Cyrillic Extended-B
        R0530_058F = 1U << 10, //Armenian
        R0590_05FF = 1U << 11, //Hebrew
        RA500_A63F = 1U << 12, //Vai
        R0600_06FF = 1U << 13, //Arabic
        R0750_077F = 1U << 13, //Arabic Supplement
        R07C0_07FF = 1U << 14, //NKo
        R0900_097F = 1U << 15, //Devanagari
        R0980_09FF = 1U << 16, //Bangla
        R0A00_0A7F = 1U << 17, //Gurmukhi
        R0A80_0AFF = 1U << 18, //Gujarati
        R0B00_0B7F = 1U << 19, //Odia
        R0B80_0BFF = 1U << 20, //Tamil
        R0C00_0C7F = 1U << 21, //Telugu
        R0C80_0CFF = 1U << 22, //Kannada
        R0D00_0D7F = 1U << 23, //Malayalam
        R0E00_0E7F = 1U << 24, //Thai
        R0E80_0EFF = 1U << 25, //Lao
        R10A0_10FF = 1U << 26, //Georgian
        R2D00_2D2F = 1U << 26, //Georgian Supplement
        R1B00_1B7F = 1U << 27, //Balinese
        R1100_11FF = 1U << 28, //Hangul Jamo
        R1E00_1EFF = 1U << 29, //Latin Extended Additional
        R2C60_2C7F = 1U << 29, //Latin Extended-C
        RA720_A7FF = 1U << 29, //Latin Extended-D
        R1F00_1FFF = 1U << 30, //Greek Extended
        R2000_206F = 1U << 31, //General Punctuation
        R2E00_2E7F = 1U << 31, //Supplemental Punctuation
    }

    [Flags]
    public enum UnicodeRange2 : uint
    {
        R2070_209F = 1U << 0, //Superscripts And Subscripts
        R20A0_20CF = 1U << 1, //Currency Symbols
        R20D0_20FF = 1U << 2, //Combining Diacritical Marks For Symbols
        R2100_214F = 1U << 3, //Letterlike Symbols
        R2150_218F = 1U << 4, //Number Forms
        R2190_21FF = 1U << 5, //Arrows
        R27F0_27FF = 1U << 5, //Supplemental Arrows-A
        R2900_297F = 1U << 5, //Supplemental Arrows-B
        R2B00_2BFF = 1U << 5, //Miscellaneous Symbols and Arrows
        R2200_22FF = 1U << 6, //Mathematical Operators
        R2A00_2AFF = 1U << 6, //Supplemental Mathematical Operators
        R27C0_27EF = 1U << 6, //Miscellaneous Mathematical Symbols-A
        R2980_29FF = 1U << 6, //Miscellaneous Mathematical Symbols-B
        R2300_23FF = 1U << 7, //Miscellaneous Technical
        R2400_243F = 1U << 8, //Control Pictures
        R2440_245F = 1U << 9, //Optical Character Recognition
        R2460_24FF = 1U << 10, //Enclosed Alphanumerics
        R2500_257F = 1U << 11, //Box Drawing
        R2580_259F = 1U << 12, //Block Elements
        R25A0_25FF = 1U << 13, //Geometric Shapes
        R2600_26FF = 1U << 14, //Miscellaneous Symbols
        R2700_27BF = 1U << 15, //Dingbats
        R3000_303F = 1U << 16, //CJK Symbols And Punctuation
        R3040_309F = 1U << 17, //Hiragana
        R30A0_30FF = 1U << 18, //Katakana
        R31F0_31FF = 1U << 18, //Katakana Phonetic Extensions
        R3100_312F = 1U << 19, //Bopomofo
        R31A0_31BF = 1U << 19, //Bopomofo Extended
        R3130_318F = 1U << 20, //Hangul Compatibility Jamo
        RA840_A87F = 1U << 21, //Phags-pa
        R3200_32FF = 1U << 22, //Enclosed CJK Letters And Months
        R3300_33FF = 1U << 23, //CJK Compatibility
        RAC00_D7AF = 1U << 24, //Hangul Syllables
        R10000_10FFFF = 1U << 25, //Non-Plane 0
        R10900_1091F = 1U << 26, //Phoenician
        R4E00_9FFF = 1U << 27, //CJK Unified Ideographs
        R2E80_2EFF = 1U << 27, //CJK Radicals Supplement
        R2F00_2FDF = 1U << 27, //Kangxi Radicals
        R2FF0_2FFF = 1U << 27, //Ideographic Description Characters
        R3400_4DBF = 1U << 27, //CJK Unified Ideographs Extension A
        R20000_2A6DF = 1U << 27, //CJK Unified Ideographs Extension B
        R3190_319F = 1U << 27, //Kanbun
        RE000_F8FF = 1U << 28, //Private Use Area (plane 0)
        R31C0_31EF = 1U << 29, //CJK Strokes
        RF900_FAFF = 1U << 29, //CJK Compatibility Ideographs
        R2F800_2FA1F = 1U << 29, //CJK Compatibility Ideographs Supplement
        RFB00_FB4F = 1U << 30, //Alphabetic Presentation Forms
        RFB50_FDFF = 1U << 31, //Arabic Presentation Forms-A
    }

    [Flags]
    public enum UnicodeRange3 : uint
    {
        RFE20_FE2F = 1U << 0, //Combining Half Marks
        RFE10_FE1F = 1U << 1, //Vertical Forms
        RFE30_FE4F = 1U << 1, //CJK Compatibility Forms
        RFE50_FE6F = 1U << 2, //Small Form Variants
        RFE70_FEFF = 1U << 3, //Arabic Presentation Forms-B
        RFF00_FFEF = 1U << 4, //Halfwidth And Fullwidth Forms
        RFFF0_FFFF = 1U << 5, //Specials
        R0F00_0FFF = 1U << 6, //Tibetan
        R0700_074F = 1U << 7, //Syriac
        R0780_07BF = 1U << 8, //Thaana
        R0D80_0DFF = 1U << 9, //Sinhala
        R1000_109F = 1U << 10, //Myanmar
        R1200_137F = 1U << 11, //Ethiopic
        R1380_139F = 1U << 11, //Ethiopic Supplement
        R2D80_2DDF = 1U << 11, //Ethiopic Extended
        R13A0_13FF = 1U << 12, //Cherokee
        R1400_167F = 1U << 13, //Unified Canadian Aboriginal Syllabics
        R1680_169F = 1U << 14, //Ogham
        R16A0_16FF = 1U << 15, //Runic
        R1780_17FF = 1U << 16, //Khmer
        R19E0_19FF = 1U << 16, //Khmer Symbols
        R1800_18AF = 1U << 17, //Mongolian
        R2800_28FF = 1U << 18, //Braille Patterns
        RA000_A48F = 1U << 19, //Yi Syllables
        RA490_A4CF = 1U << 19, //Yi Radicals
        R1700_171F = 1U << 20, //Tagalog
        R1720_173F = 1U << 20, //Hanunoo
        R1740_175F = 1U << 20, //Buhid
        R1760_177F = 1U << 20, //Tagbanwa
        R10300_1032F = 1U << 21, //Old Italic
        R10330_1034F = 1U << 22, //Gothic
        R10400_1044F = 1U << 23, //Deseret
        R1D000_1D0FF = 1U << 24, //Byzantine Musical Symbols
        R1D100_1D1FF = 1U << 24, //Musical Symbols
        R1D200_1D24F = 1U << 24, //Ancient Greek Musical Notation
        R1D400_1D7FF = 1U << 25, //Mathematical Alphanumeric Symbols
        RF0000_FFFFD = 1U << 26, //Private Use (plane 15)
        R100000_10FFFD = 1U << 26, //Private Use (plane 16)
        RFE00_FE0F = 1U << 27, //Variation Selectors
        RE0100_E01EF = 1U << 27, //Variation Selectors Supplement
        RE0000_E007F = 1U << 28, //Tags
        R1900_194F = 1U << 29, //Limbu
        R1950_197F = 1U << 30, //Tai Le
        R1980_19DF = 1U << 31, //New Tai Lue
    }

    [Flags]
    public enum UnicodeRange4 : uint
    {
        R1A00_1A1F = 1U << 0, //Buginese
        R2C00_2C5F = 1U << 1, //Glagolitic
        R2D30_2D7F = 1U << 2, //Tifinagh
        R4DC0_4DFF = 1U << 3, //Yijing Hexagram Symbols
        RA800_A82F = 1U << 4, //Syloti Nagri
        R10000_1007F = 1U << 5, //Linear B Syllabary
        R10080_100FF = 1U << 5, //Linear B Ideograms
        R10100_1013F = 1U << 5, //Aegean Numbers
        R10140_1018F = 1U << 6, //Ancient Greek Numbers
        R10380_1039F = 1U << 7, //Ugaritic
        R103A0_103DF = 1U << 8, //Old Persian
        R10450_1047F = 1U << 9, //Shavian
        R10480_104AF = 1U << 10, //Osmanya
        R10800_1083F = 1U << 11, //Cypriot Syllabary
        R10A00_10A5F = 1U << 12, //Kharoshthi
        R1D300_1D35F = 1U << 13, //Tai Xuan Jing Symbols
        R12000_123FF = 1U << 14, //Cuneiform
        R12400_1247F = 1U << 14, //Cuneiform Numbers and Punctuation
        R1D360_1D37F = 1U << 15, //Counting Rod Numerals
        R1B80_1BBF = 1U << 16, //Sundanese
        R1C00_1C4F = 1U << 17, //Lepcha
        R1C50_1C7F = 1U << 18, //Ol Chiki
        RA880_A8DF = 1U << 19, //Saurashtra
        RA900_A92F = 1U << 20, //Kayah Li
        RA930_A95F = 1U << 21, //Rejang
        RAA00_AA5F = 1U << 22, //Cham
        R10190_101CF = 1U << 23, //Ancient Symbols
        R101D0_101FF = 1U << 24, //Phaistos Disc
        R102A0_102DF = 1U << 25, //Carian
        R10280_1029F = 1U << 25, //Lycian
        R10920_1093F = 1U << 25, //Lydian
        R1F030_1F09F = 1U << 26, //Domino Tiles
        R1F000_1F02F = 1U << 26, //Mahjong Tiles
    }

    [Flags]
    public enum CodePageRange1 : uint
    {
        LATIN1_1252 = 1U << 0, // Latin 1
        LATIN2_1250 = 1U << 1, // Latin 2: Eastern Europe
        CYRILLIC_1251 = 1U << 2, // Cyrillic
        GREEK_1253 = 1U << 3, // Greek
        TURKISH_1254 = 1U << 4, // Turkish
        HEBREW_1255 = 1U << 5, // Hebrew
        ARABIC_1256 = 1U << 6, // Arabic
        BALTIC_1257 = 1U << 7, // Windows Baltic
        VIETNAMESE_1258 = 1U << 8, // Vietnamese
        THAI_874 = 1U << 16, // Thai
        JIS_932 = 1U << 17, // JIS/Japan
        CHINESE_936 = 1U << 18, // Chinese: Simplified chars—PRC and Singapore
        KOREAN_949 = 1U << 19, // Korean Wansung
        CHINESE_950 = 1U << 20, // Chinese: Traditional chars—Taiwan and Hong Kong SAR
        KOREAN_1361 = 1U << 21, // Korean Johab
        MACINTOSH = 1U << 29, // Macintosh Character Set (US Roman)
        OEM = 1U << 30, // OEM Character Set
        SYMBOL = 1U << 31, // Symbol Character Set
    }

    [Flags]
    public enum CodePageRange2 : uint
    {
        IBM_869 = 1U << 16, // IBM Greek
        MSDOS_RUSSIAN_866 = 1U << 17, // MS-DOS Russian
        MSDOS_NORDIC_865 = 1U << 18, // MS-DOS Nordic
        ARABIC_864 = 1U << 19, // Arabic
        MSDOS_CANADAFRENCH_863 = 1U << 20, // MS-DOS Canadian French
        HEBREW_862 = 1U << 21, // Hebrew
        MSDOS_ICELANDIC_861 = 1U << 22, // MS-DOS Icelandic
        MSDOS_PROTUGUESE_860 = 1U << 23, // MS-DOS Portuguese
        IBM_TURKISH_857 = 1U << 24, // IBM Turkish
        IBM_CYRILLIC_855 = 1U << 25, // IBM Cyrillic; primarily Russian
        LATIN2_852 = 1U << 26, // Latin 2
        MSDOS_BALTIC_775 = 1U << 27, // MS-DOS Baltic
        GREEK_737 = 1U << 28, // Greek; former 437 G
        ARABIC_708 = 1U << 29, // Arabic; ASMO 708
        WE_LATIN1_850 = 1U << 30, // WE/Latin 1
        US_437 = 1U << 31, // US
    }

    [Flags]
    public enum SimpleGlyphFlags : byte
    {
        ON_CURVE_POINT = 0x01, // Bit 0: If set, the point is on the curve; otherwise, it is off the curve.
        X_SHORT_VECTOR = 0x02, // Bit 1: If set, the corresponding x-coordinate is 1 byte long, and the sign is determined by the X_IS_SAME_OR_POSITIVE_X_SHORT_VECTOR flag.If not set, its interpretation depends on the X_IS_SAME_OR_POSITIVE_X_SHORT_VECTOR flag: If that other flag is set, the x-coordinate is the same as the previous x-coordinate, and no element is added to the xCoordinates array.If both flags are not set, the corresponding element in the xCoordinates array is two bytes and interpreted as a signed integer.See the description of the X_IS_SAME_OR_POSITIVE_X_SHORT_VECTOR flag for additional information.
        Y_SHORT_VECTOR = 0x04, // Bit 2: If set, the corresponding y-coordinate is 1 byte long, and the sign is determined by the Y_IS_SAME_OR_POSITIVE_Y_SHORT_VECTOR flag.If not set, its interpretation depends on the Y_IS_SAME_OR_POSITIVE_Y_SHORT_VECTOR flag: If that other flag is set, the y-coordinate is the same as the previous y-coordinate, and no element is added to the yCoordinates array.If both flags are not set, the corresponding element in the yCoordinates array is two bytes and interpreted as a signed integer.See the description of the Y_IS_SAME_OR_POSITIVE_Y_SHORT_VECTOR flag for additional information.
        REPEAT_FLAG = 0x08, // Bit 3: If set, the next byte (read as unsigned) specifies the number of additional times this flag byte is to be repeated in the logical flags array — that is, the number of additional logical flag entries inserted after this entry. (In the expanded logical array, this bit is ignored.) In this way, the number of flags listed can be smaller than the number of points in the glyph description.
        X_IS_SAME_OR_POSITIVE_X_SHORT_VECTOR = 0x10, // Bit 4: This flag has two meanings, depending on how the X_SHORT_VECTOR flag is set. If X_SHORT_VECTOR is set, this bit describes the sign of the value, with 1 equaling positive and 0 negative.If X_SHORT_VECTOR is not set and this bit is set, then the current x-coordinate is the same as the previous x-coordinate.If X_SHORT_VECTOR is not set and this bit is also not set, the current x - coordinate is a signed 16 - bit delta vector.
        Y_IS_SAME_OR_POSITIVE_Y_SHORT_VECTOR = 0x20, // Bit 5: This flag has two meanings, depending on how the Y_SHORT_VECTOR flag is set.If Y_SHORT_VECTOR is set, this bit describes the sign of the value, with 1 equaling positive and 0 negative.If Y_SHORT_VECTOR is not set and this bit is set, then the current y-coordinate is the same as the previous y-coordinate.If Y_SHORT_VECTOR is not set and this bit is also not set, the current y - coordinate is a signed 16 - bit delta vector.
        OVERLAP_SIMPLE = 0x40, // Bit 6: If set, contours in the glyph description could overlap.Use of this flag is not required — that is, contours may overlap without having this flag set. When used, it must be set on the first flag byte for the glyph. See additional details below.
    }

    [Flags]
    public enum ComponentGlyphFlags : ushort
    {
        ARG_1_AND_2_ARE_WORDS = 0x0001, // Bit 0: If this is set, the arguments are 16-bit (uint16 or int16); otherwise, they are bytes (uint8 or int8).
        ARGS_ARE_XY_VALUES = 0x0002, // Bit 1: If this is set, the arguments are signed xy values; otherwise, they are unsigned point numbers.
        ROUND_XY_TO_GRID = 0x0004, // Bit 2: If set and ARGS_ARE_XY_VALUES is also set, the xy values are rounded to the nearest grid line. Ignored if ARGS_ARE_XY_VALUES is not set.
        WE_HAVE_A_SCALE = 0x0008, // Bit 3: This indicates that there is a simple scale for the component. Otherwise, scale = 1.0.
        MORE_COMPONENTS = 0x0020, // Bit 5: Indicates at least one more glyph after this one.
        WE_HAVE_AN_X_AND_Y_SCALE = 0x0040, // Bit 6: The x direction will use a different scale from the y direction.
        WE_HAVE_A_TWO_BY_TWO = 0x0080, // Bit 7: There is a 2 by 2 transformation that will be used to scale the component.
        WE_HAVE_INSTRUCTIONS = 0x0100, // Bit 8: Following the last component are instructions for the composite glyph.
        USE_MY_METRICS = 0x0200, // Bit 9: If set, this forces the aw and lsb(and rsb) for the composite to be equal to those from this component glyph.This works for hinted and unhinted glyphs.
        OVERLAP_COMPOUND = 0x0400, // Bit 10: If set, the components of the compound glyph overlap.Use of this flag is not required — that is, component glyphs may overlap without having this flag set.When used, it must be set on the flag word for the first component.Some rasterizer implementations may require fonts to use this flag to obtain correct behavior — see additional remarks, above, for the similar OVERLAP_SIMPLE flag used in simple - glyph descriptions.
        SCALED_COMPONENT_OFFSET = 0x0800, // Bit 11: The composite is designed to have the component offset scaled.Ignored if ARGS_ARE_XY_VALUES is not set.
        UNSCALED_COMPONENT_OFFSET = 0x1000, // Bit 12: The composite is designed not to have the component offset scaled. Ignored if ARGS_ARE_XY_VALUES is not set.
    }

    [Flags]
    public enum KernCoverage : ushort
    {
        Horizontal = 1<<0,
        Minimum = 1<<1,
        CrossStream = 1<<2,
        Override = 1<<3,

        FormatMask = 0xFF00
    }

}
	 
	 
   
   
   
      
     
    
   
    
   
   