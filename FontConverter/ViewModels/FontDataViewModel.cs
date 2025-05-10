using LVGLFontConverter.Contracts.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LVGLFontConverter.ViewModels;

public class FontDataViewModel : BaseViewModel
{
    public FontDataViewModel()
    {
        _FullFontName = string.Empty;
        _FontFamily = string.Empty;
        _FontSubfamily = string.Empty;
        _Manufacturer = string.Empty;
        _FontRevision = 0.0;
        _Created = new();
        _Modified = new();
        _AdvanceWidthMax = 0;
        _Ascent = 0;
        _Descent = 0;
        _XMin = 0;
        _YMin = 0;
        _XMax = 0;
        _YMax = 0;
        _MaxCharWidth = 0;
    }

    #region Private Properties
    private string _FullFontName;
    private string _FontFamily;
    private string _FontSubfamily;
    private string _Manufacturer;
    private double _FontRevision;
    private DateTime _Created;
    private DateTime _Modified;
    private int _AdvanceWidthMax;
    private int _Ascent;
    private int _Descent;
    private int _XMin;
    private int _YMin;
    private int _XMax;
    private int _YMax;
    private int _MaxCharWidth;
    #endregion Private Properties

    #region Public Properties
    public string FullFontName
    {
        get { return _FullFontName; }
        set { SetProperty(ref _FullFontName, value); }
    }
    public string FontFamily
    {
        get { return _FontFamily; }
        set { SetProperty(ref _FontFamily, value); }
    }
    public string FontSubfamily
    {
        get { return _FontSubfamily; }
        set { SetProperty(ref _FontSubfamily, value); }
    }
    public string Manufacturer
    {
        get { return _Manufacturer; }
        set { SetProperty(ref _Manufacturer, value); }
    }
    public double FontRevision
    {
        get { return _FontRevision; }
        set { SetProperty(ref _FontRevision, value); }
    }
    public DateTime Created
    {
        get { return _Created; }
        set { SetProperty(ref _Created, value); }
    }
    public DateTime Modified
    {
        get { return _Modified; }
        set { SetProperty(ref _Modified, value); }
    }
    public int AdvanceWidthMax
    {
        get { return _AdvanceWidthMax; }
        set { SetProperty(ref _AdvanceWidthMax, value); }
    }
    public int Ascent
    {
        get { return _Ascent; }
        set { SetProperty(ref _Ascent, value); }
    }
    public int Descent
    {
        get { return _Descent; }
        set { SetProperty(ref _Descent, value); }
    }
    public int XMin
    {
        get { return _XMin; }
        set { SetProperty(ref _XMin, value); }
    }
    public int YMin
    {
        get { return _YMin; }
        set { SetProperty(ref _YMin, value); }
    }
    public int XMax
    {
        get { return _XMax; }
        set { SetProperty(ref _XMax, value); }
    }
    public int YMax
    {
        get { return _YMax; }
        set { SetProperty(ref _YMax, value); }
    }
    public int MaxCharWidth
    {
        get { return _MaxCharWidth; }
        set { SetProperty(ref _MaxCharWidth, value); }
    }
    #endregion Public Properties
}
