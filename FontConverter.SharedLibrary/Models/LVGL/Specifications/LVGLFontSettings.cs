using static FontConverter.SharedLibrary.Helpers.LVGLFontEnums;

namespace FontConverter.SharedLibrary.Models;

public class LVGLFontSettings
{
    public LVGLFontSettings()
    {
        FontName = string.Empty;
        FontBitPerPixel = BIT_PER_PIXEL_ENUM.BPP_8;
        FontSize = 12;   
        FontSubPixel = SUB_Pixel_ENUM.SUB_PIXEL_NONE;
        Fallback = string.Empty;
    }

    public string FontName { get; set; }
    public BIT_PER_PIXEL_ENUM FontBitPerPixel { get; set; }
    public int FontSize { get; set; }
    public SUB_Pixel_ENUM FontSubPixel { get; set; }
    public string Fallback { get; set; }
}
