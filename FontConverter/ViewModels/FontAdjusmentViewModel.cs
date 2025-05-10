using LVGLFontConverter.Contracts.ViewModels;
using LVGLFontConverter.Helpers;
using LVGLFontConverter.Models;
using System.Collections.ObjectModel;
using static LVGLFontConverter.Library.Helpers.LVGLFontEnums;

namespace LVGLFontConverter.ViewModels;

public class FontAdjusmentViewModel : BaseViewModel
{
    public FontAdjusmentViewModel()
    {
        FillGlyphStyleList();
        _AntiAlias = true;
        _Dither = true;
        _ColorFilter = true;
        _Shader = true;
        _StyleSelectedIndex = 0;
        _Style = GlyphStyleList[_StyleSelectedIndex].Style;
        _Gamma = 50;
        _Threshold = 0;
    }

    #region Private Properties
    private bool _AntiAlias;
    private bool _Dither;
    private bool _ColorFilter;
    private bool _Shader;
    private int _StyleSelectedIndex;
    private GLYPH_STYLE _Style;
    private int _Gamma;
    private int _Threshold;
    #endregion Private Properties

    #region Public Properties
    public ObservableCollection<GlyphStyle> GlyphStyleList { get; } = [];
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
    public int StyleSelectedIndex
    {
        get { return _StyleSelectedIndex; }
        set { SetProperty(ref _StyleSelectedIndex, value); }
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
    #endregion Public Properties

    #region Private Methods
    private void FillGlyphStyleList()
    {
        GlyphStyleList.Add(
        new GlyphStyle()
        {
            Style = GLYPH_STYLE.STYLE_FILL,
            Description = "Text_Item_FontAdjusment_GlyphStyle_Fill".GetLocalized(),
        });
        GlyphStyleList.Add(
        new GlyphStyle()
        {
            Style = GLYPH_STYLE.STYLE_STROKE,
            Description = "Text_Item_FontAdjusment_GlyphStyle_Stroke".GetLocalized(),
        });
        GlyphStyleList.Add(
        new GlyphStyle()
        {
            Style = GLYPH_STYLE.STYLE_FILL_STROKE,
            Description = "Text_Item_FontAdjusment_GlyphStyle_FillStroke".GetLocalized(),
        });
    }
    #endregion Private Methods
}
