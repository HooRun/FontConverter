using System.Text.RegularExpressions;
using static FontConverter.SharedLibrary.Helpers.LVGLFontEnums;

namespace FontConverter.Blazor.ViewModels;

public class FontSettingsViewModel : BaseViewModel
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
        set { SetProperty(ref _FontName, value); }
    }
    public BIT_PER_PIXEL_ENUM FontBitPerPixel
    {
        get { return _FontBitPerPixel; }
        set { SetProperty(ref _FontBitPerPixel, value); }
    }
    public int FontSize
    {
        get { return _FontSize; }
        set { SetProperty(ref _FontSize, value); }
    }
    public SUB_Pixel_ENUM FontSubPixel
    {
        get { return _FontSubPixel; }
        set { SetProperty(ref _FontSubPixel, value); }
    }
    public string Fallback
    {
        get { return _Fallback; }
        set { SetProperty(ref _Fallback, value); }
    }


    public void CleanData()
    {
        FontName = string.Empty;
        FontBitPerPixel = BIT_PER_PIXEL_ENUM.BPP_8;
        FontSize = 12;
        FontSubPixel = SUB_Pixel_ENUM.SUB_PIXEL_NONE;
        Fallback = string.Empty;
    }
    
}
