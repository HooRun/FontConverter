using LVGLFontConverter.Contracts.ViewModels;
using LVGLFontConverter.Helpers;
using LVGLFontConverter.Models;
using System.Collections.ObjectModel;
using static LVGLFontConverter.Library.Helpers.LVGLFontEnums;

namespace LVGLFontConverter.ViewModels;

public class FontPropertiesViewModel : BaseViewModel
{
    public FontPropertiesViewModel()
    {
        FillFontBitPerPixelList();
        FillFontSubPixellList();

        _FontName = string.Empty;
        _FontBitPerPixelSelectedIndex = 3;
        _FontBitPerPixel = FontBitPerPixelList[FontBitPerPixelSelectedIndex].BPP;
        _FontSize = 8;
        _MaxFontSize = 128;
        _MinFontSize = 8;
        _LineHeight = _FontSize;
        _MaxLineHeight = 200;
        _MinLineHeight = 8;
        _BaseLine = 0;
        _MaxBaseLine = 200;
        _MinBaseLine = 0;
        _YAxisPosition = 0;
        _MaxYAxisPosition = 0;
        _MinYAxisPosition = 0;
        _UnderlinePosition = 0;
        _MaxUnderLinePosition = 100;
        _MinUnderLinePosition = 0;
        _UnderlineThickness = 0;
        _MaxUnderlineThickness = 100;
        _MinUnderlineThickness = 0;
        _FontSubPixelSelectedIndex = 0;
        _FontSubPixel = FontSubPixelList[FontSubPixelSelectedIndex].SubPixel;
        _Fallback = string.Empty;
    }

    #region Private Properties
    private string _FontName;
    private int _FontBitPerPixelSelectedIndex;
    private BIT_PER_PIXEL_ENUM _FontBitPerPixel;
    private int _FontSize;
    private int _MaxFontSize;
    private int _MinFontSize;
    private int _LineHeight;
    private int _MaxLineHeight;
    private int _MinLineHeight;
    private int _BaseLine;
    private int _MaxBaseLine;
    private int _MinBaseLine;
    private int _YAxisPosition;
    private int _MaxYAxisPosition;
    private int _MinYAxisPosition;
    private int _UnderlinePosition;
    private int _MaxUnderLinePosition;
    private int _MinUnderLinePosition;
    private int _UnderlineThickness;
    private int _MaxUnderlineThickness;
    private int _MinUnderlineThickness;
    private int _FontSubPixelSelectedIndex;
    private SUB_Pixel_ENUM _FontSubPixel;
    private string _Fallback;
    #endregion Private Properties

    #region Public Properties
    public ObservableCollection<FontBitPerPixel> FontBitPerPixelList { get; } = [];
    public ObservableCollection<FontSubPixel> FontSubPixelList { get; } = [];
    public string FontName
	{
		get { return _FontName; }
		set { SetProperty(ref _FontName, value); }
	}
    public int FontBitPerPixelSelectedIndex
    {
        get { return _FontBitPerPixelSelectedIndex; }
        set { SetProperty(ref _FontBitPerPixelSelectedIndex, value); }
    }
    public BIT_PER_PIXEL_ENUM FontBitPerPixel
    {
        get { return _FontBitPerPixel; }
        set { SetProperty(ref _FontBitPerPixel, value); }
    }
 	public int FontSize
	{
		get { return _FontSize; }
		set 
        {
            if (value <= MinFontSize)
            {
                _FontSize = MinFontSize - 1;
            }
            SetProperty(ref _FontSize, value); 
        }
	}
    public int MaxFontSize
    {
        get { return _MaxFontSize; }
        set { SetProperty(ref _MaxFontSize, value); ; }
    }
    public int MinFontSize
    {
        get { return _MinFontSize; }
        set { SetProperty(ref _MinFontSize, value); }
    }
    public int LineHeight
    {
        get { return _LineHeight; }
        set 
        {
            if (value <= MinLineHeight)
            {
                _LineHeight = MinLineHeight - 1;
            }
            SetProperty(ref _LineHeight, value); 
        }
    }
    public int MaxLineHeight
    {
        get { return _MaxLineHeight; }
        set { SetProperty(ref _MaxLineHeight, value); }
    }
    public int MinLineHeight
    {
        get { return _MinLineHeight; }
        set { SetProperty(ref _MinLineHeight, value); }
    }
    public int BaseLine
    {
        get { return _BaseLine; }
        set 
        {
            if (value <= MinBaseLine)
            {
                _BaseLine = MinBaseLine - 1;
            }
            SetProperty(ref _BaseLine, value); 
        }
    }
    public int MaxBaseLine
    {
        get { return _MaxBaseLine; }
        set { SetProperty(ref _MaxBaseLine, value); }
    }
    public int MinBaseLine
    {
        get { return _MinBaseLine; }
        set { SetProperty(ref _MinBaseLine, value); }
    }
    public int YAxisPosition
    {
        get { return _YAxisPosition; }
        set
        {
            if (value <= MinYAxisPosition)
            {
                _YAxisPosition = MinYAxisPosition - 1;
            }
            SetProperty(ref _YAxisPosition, value);
        }
    }
    public int MaxYAxisPosition
    {
        get { return _MaxYAxisPosition; }
        set { SetProperty(ref _MaxYAxisPosition, value); }
    }
    public int MinYAxisPosition
    {
        get { return _MinYAxisPosition; }
        set { SetProperty(ref _MinYAxisPosition, value); }
    }
    public int UnderlinePosition
    {
        get { return _UnderlinePosition; }
        set 
        {
            if (value <= MinUnderLinePosition)
            {
                _UnderlinePosition = MinUnderLinePosition - 1;
            }
            SetProperty(ref _UnderlinePosition, value); 
        }
    }
    public int MaxUnderLinePosition
    {
        get { return _MaxUnderLinePosition; }
        set { SetProperty(ref _MaxUnderLinePosition, value); }
    }
    public int MinUnderLinePosition
    {
        get { return _MinUnderLinePosition; }
        set { SetProperty(ref _MinUnderLinePosition, value); }
    }
    public int UnderlineThickness
    {
        get { return _UnderlineThickness; }
        set 
        {
            if ( value <= MinUnderlineThickness)
            {
                _UnderlineThickness = MinUnderlineThickness - 1;
            }
            SetProperty(ref _UnderlineThickness, value); 
        }
    }
    public int MaxUnderlineThickness
    {
        get { return _MaxUnderlineThickness; }
        set { SetProperty(ref _MaxUnderlineThickness, value); }
    }
    public int MinUnderlineThickness
    {
        get { return _MinUnderlineThickness; }
        set { SetProperty(ref _MinUnderlineThickness, value); }
    }
    public int FontSubPixelSelectedIndex
    {
        get { return _FontSubPixelSelectedIndex; }
        set { SetProperty(ref _FontSubPixelSelectedIndex, value); }
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
    #endregion Public Properties

    #region Private Methods
    private void FillFontBitPerPixelList()
    {
        FontBitPerPixelList.Add(
        new FontBitPerPixel()
        {
            BPP = BIT_PER_PIXEL_ENUM.BPP_1,
            Description = "Text_Item_FontProperties_BitPerPixel_1".GetLocalized(),
        });
        FontBitPerPixelList.Add(
        new FontBitPerPixel()
        {
            BPP = BIT_PER_PIXEL_ENUM.BPP_2,
            Description = "Text_Item_FontProperties_BitPerPixel_2".GetLocalized(),
        });
        FontBitPerPixelList.Add(
        new FontBitPerPixel()
        {
            BPP = BIT_PER_PIXEL_ENUM.BPP_4,
            Description = "Text_Item_FontProperties_BitPerPixel_4".GetLocalized(),
        });
        FontBitPerPixelList.Add(
        new FontBitPerPixel()
        {
            BPP = BIT_PER_PIXEL_ENUM.BPP_8,
            Description = "Text_Item_FontProperties_BitPerPixel_8".GetLocalized(),
        });

    }

    private void FillFontSubPixellList()
    {
        FontSubPixelList.Add(
        new FontSubPixel()
        {
            SubPixel = SUB_Pixel_ENUM.SUB_PIXEL_NONE,
            Description = "Text_Item_FontProperties_SubPixel_None".GetLocalized()
        });
        FontSubPixelList.Add(
        new FontSubPixel()
        {
            SubPixel = SUB_Pixel_ENUM.SUB_PIXEL_Horizontal,
            Description = "Text_Item_FontProperties_SubPixel_Horizontal".GetLocalized()
        });
        FontSubPixelList.Add(
        new FontSubPixel()
        {
            SubPixel = SUB_Pixel_ENUM.SUB_PIXEL_Vertical,
            Description = "Text_Item_FontProperties_SubPixel_Vertical".GetLocalized()
        });
        FontSubPixelList.Add(
        new FontSubPixel()
        {
            SubPixel = SUB_Pixel_ENUM.SUB_PIXEL_Both,
            Description = "Text_Item_FontProperties_SubPixel_Both".GetLocalized()
        });
    }
    #endregion Private Methods
}
