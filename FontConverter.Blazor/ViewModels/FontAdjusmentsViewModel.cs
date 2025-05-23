using FontConverter.SharedLibrary.Models;
using Microsoft.AspNetCore.Components;
using static FontConverter.SharedLibrary.Helpers.LVGLFontEnums;

namespace FontConverter.Blazor.ViewModels;

public class FontAdjusmentsViewModel
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
        set
        {
            if (value == _AntiAlias)
                return;
            _AntiAlias = value;
        }
    }
    public bool Dither
    {
        get { return _Dither; }
        set
        {
            if (value == _Dither)
                return;
            _Dither = value;
        }
    }
    public bool ColorFilter
    {
        get { return _ColorFilter; }
        set
        {
            if (value == _ColorFilter)
                return;
            _ColorFilter = value;
        }
    }
    public bool Shader
    {
        get { return _Shader; }
        set
        {
            if (value == _Shader)
                return;
            _Shader = value;
        }
    }
    public GLYPH_STYLE Style
    {
        get { return _Style; }
        set
        {
            if (value == _Style)
                return;
            _Style = value;
        }
    }
    public int Gamma
    {
        get { return _Gamma; }
        set
        {
            if (value == _Gamma)
                return;
            _Gamma = value;
        }
    }
    public int Threshold
    {
        get { return _Threshold; }
        set
        {
            if (value == _Threshold)
                return;
            _Threshold = value;
        }
    }

}
