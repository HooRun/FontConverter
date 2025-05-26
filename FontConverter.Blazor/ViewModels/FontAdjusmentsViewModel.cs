using FontConverter.SharedLibrary.Models;
using Microsoft.AspNetCore.Components;
using static FontConverter.SharedLibrary.Helpers.LVGLFontEnums;

namespace FontConverter.Blazor.ViewModels;

public class FontAdjusmentsViewModel : BaseViewModel
{
    
    public FontAdjusmentsViewModel()
    {
        _AntiAlias = true;
        _Dither = true;
        _ColorFilter = true;
        _Shader = true;
        _Style = GLYPH_STYLE.STYLE_FILL;
        _Gamma = 50;
        _Threshold = 0;
    }


    private bool _AntiAlias;
    private bool _Dither;
    private bool _ColorFilter;
    private bool _Shader;
    private GLYPH_STYLE _Style;
    private int _Gamma;
    private int _Threshold;

    public bool AntiAlias
    {
        get { return _AntiAlias; }
        set { SetProperty(ref _AntiAlias, value); }
    }
    public bool Dither
    {
        get { return _Dither; }
        set { SetProperty(ref _Dither, value); }
    }
    public bool ColorFilter
    {
        get { return _ColorFilter; }
        set { SetProperty(ref _ColorFilter, value); }
    }
    public bool Shader
    {
        get { return _Shader; }
        set { SetProperty(ref _Shader, value); }
    }
    public GLYPH_STYLE Style
    {
        get { return _Style; }
        set { SetProperty(ref _Style, value); }
    }
    public int Gamma
    {
        get { return _Gamma; }
        set { SetProperty(ref _Gamma, value); }
    }
    public int Threshold
    {
        get { return _Threshold; }
        set { SetProperty(ref _Threshold, value); }
    }

    public void CleanData()
    {
        AntiAlias = true;
        Dither = true;
        ColorFilter = true;
        Shader = true;
        Style = GLYPH_STYLE.STYLE_FILL;
        Gamma = 50;
        Threshold = 0;
    }

}
