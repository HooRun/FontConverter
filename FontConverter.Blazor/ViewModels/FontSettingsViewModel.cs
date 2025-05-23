using System.Text.RegularExpressions;
using static FontConverter.SharedLibrary.Helpers.LVGLFontEnums;

namespace FontConverter.Blazor.ViewModels;

public class FontSettingsViewModel
{
    public FontSettingsViewModel()
    {
        _FontName = string.Empty;
        _FontBitPerPixel = BIT_PER_PIXEL_ENUM.BPP_8;
        _FontSize = 12;
        _FontSubPixel = SUB_Pixel_ENUM.SUB_PIXEL_NONE;
        _Fallback = string.Empty;
    }

    private string _FontName;
    private BIT_PER_PIXEL_ENUM _FontBitPerPixel;
    private int _FontSize = 12;
    private SUB_Pixel_ENUM _FontSubPixel;
    private string _Fallback;

    public string FontName 
    { 
        get { return _FontName; } 
        set 
        {
            if (value == _FontName)
                return;
            _FontName = value; 
        }
    }
    public BIT_PER_PIXEL_ENUM FontBitPerPixel
    {
        get { return _FontBitPerPixel; }
        set 
        {
            if (value == _FontBitPerPixel)
                return;
            _FontBitPerPixel = value; 
        }
    }
    public int FontSize
    {
        get { return _FontSize; }
        set 
        {
            if (value == _FontSize)
                return;
            _FontSize = value; 
        }
    }
    public SUB_Pixel_ENUM FontSubPixel
    {
        get { return _FontSubPixel; }
        set 
        {
            if (value == _FontSubPixel)
                return;
            _FontSubPixel = value; 
        }
    }
    public string Fallback
    {
        get { return _Fallback; }
        set 
        {
            if (value == _Fallback)
                return;
            _Fallback = value; 
        }
    }

    
}
