namespace FontConverter.SharedLibrary.Helpers;

public static class LVGLFontEnums
{
    [Flags]
    public enum BIT_PER_PIXEL_ENUM : int
    {
        BPP_1 = 0x0001,
        BPP_2 = 0x0002,
        BPP_4 = 0x0004,
        BPP_8 = 0x0008,
    }

    [Flags]
    public enum SUB_Pixel_ENUM : int
    {
        SUB_PIXEL_NONE = 0x00,
        SUB_PIXEL_Horizontal = 0x01,
        SUB_PIXEL_Vertical = 0x02,
        SUB_PIXEL_Both = 0x04,
    }

    [Flags]
    public enum GLYPH_STYLE : int
    {
        STYLE_FILL = 0x00,
        STYLE_STROKE = 0x01,
        STYLE_FILL_STROKE = 0x02,
    }
}
