namespace FontConverter.Blazor.ViewModels;

public class FontInformationsViewModel : BaseViewModel
{
    public FontInformationsViewModel()
    {
        _FontName = string.Empty;
        _LineHeight = string.Empty;
        _BaseLine = string.Empty;
        _CharWidthMax = string.Empty;
        _AdvanceWidthMax = string.Empty;
        _UnderlinePosition = string.Empty;
        _UnderlineThickness = string.Empty;
        _Ascent = string.Empty;
        _Descent = string.Empty;
        _XMin = string.Empty;
        _YMin = string.Empty;
        _XMax = string.Empty;
        _YMax = string.Empty;
    }

    private string _FontName;
    private string _LineHeight;
    private string _BaseLine;
    private string _CharWidthMax;
    private string _AdvanceWidthMax;
    private string _UnderlinePosition;
    private string _UnderlineThickness;
    private string _Ascent;
    private string _Descent;
    private string _XMin;
    private string _YMin;
    private string _XMax;
    private string _YMax;

    public string FontName
    {
        get { return _FontName; }
        set { SetProperty(ref _FontName, value); }
    }
    public string LineHeight
    {
        get { return _LineHeight; }
        set { SetProperty(ref _LineHeight, value); }
    }
    public string BaseLine
    {
        get { return _BaseLine; }
        set { SetProperty(ref _BaseLine, value); }
    }
    public string CharWidthMax
    {
        get { return _CharWidthMax; }
        set { SetProperty(ref _CharWidthMax, value); }
    }
    public string AdvanceWidthMax
    {
        get { return _AdvanceWidthMax; }
        set { SetProperty(ref _AdvanceWidthMax, value); }
    }
    public string UnderlinePosition
    {
        get { return _UnderlinePosition; }
        set { SetProperty(ref _UnderlinePosition, value); }
    }
    public string UnderlineThickness
    {
        get { return _UnderlineThickness; }
        set { SetProperty(ref _UnderlineThickness, value); }
    }
    public string Ascent
    {
        get { return _Ascent; }
        set { SetProperty(ref _Ascent, value); }
    }
    public string Descent
    {
        get { return _Descent; }
        set { SetProperty(ref _Descent, value); }
    }
    public string XMin
    {
        get { return _XMin; }
        set { SetProperty(ref _XMin, value); }
    }
    public string YMin
    {
        get { return _YMin; }
        set { SetProperty(ref _YMin, value); }
    }
    public string XMax
    {
        get { return _XMax; }
        set { SetProperty(ref _XMax, value); }
    }
    public string YMax
    {
        get { return _YMax; }
        set { SetProperty(ref _YMax, value); }
    }

    public void CleanData()
    {
        FontName = string.Empty;
        LineHeight = string.Empty;
        BaseLine = string.Empty;
        CharWidthMax = string.Empty;
        AdvanceWidthMax = string.Empty;
        UnderlinePosition = string.Empty;
        UnderlineThickness = string.Empty;
        Ascent = string.Empty;
        Descent = string.Empty;
        XMin = string.Empty;
        YMin = string.Empty;
        XMax = string.Empty;
        YMax = string.Empty;
    }
}
