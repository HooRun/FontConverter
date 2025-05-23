namespace FontConverter.Blazor.ViewModels;

public class FontInformationsViewModel
{
    public FontInformationsViewModel()
    {
        _FontName = string.Empty;
        _LineHeight = 0;
        _BaseLine = 0;
        _CharWidthMax = 0;
        _AdvanceWidthMax = 0;
        _UnderlinePosition = 0;
        _UnderlineThickness = 0;
        _Ascent = 0;
        _Descent = 0;
        _XMin = 0;
        _YMin = 0;
        _XMax = 0;
        _YMax = 0;
    }

    private string _FontName;
    private int _LineHeight;
    private int _BaseLine;
    private int _CharWidthMax;
    private int _AdvanceWidthMax;
    private int _UnderlinePosition;
    private int _UnderlineThickness;
    private int _Ascent;
    private int _Descent;
    private int _XMin;
    private int _YMin;
    private int _XMax;
    private int _YMax;

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
    public int LineHeight
    {
        get { return _LineHeight; }
        set
        {
            if (value == _LineHeight)
                return;
            _LineHeight = value;
        }
    }
    public int BaseLine
    {
        get { return _BaseLine; }
        set
        {
            if (value == _BaseLine)
                return;
            _BaseLine = value;
        }
    }
    public int CharWidthMax
    {
        get { return _CharWidthMax; }
        set
        {
            if (value == _CharWidthMax)
                return;
            _CharWidthMax = value;
        }
    }
    public int AdvanceWidthMax
    {
        get { return _AdvanceWidthMax; }
        set
        {
            if (value == _AdvanceWidthMax)
                return;
            _AdvanceWidthMax = value;
        }
    }
    public int UnderlinePosition
    {
        get { return _UnderlinePosition; }
        set
        {
            if (value == _UnderlinePosition)
                return;
            _UnderlinePosition = value;
        }
    }
    public int UnderlineThickness
    {
        get { return _UnderlineThickness; }
        set
        {
            if (value == _UnderlineThickness)
                return;
            _UnderlineThickness = value;
        }
    }
    public int Ascent
    {
        get { return _Ascent; }
        set
        {
            if (value == _Ascent)
                return;
            _Ascent = value;
        }
    }
    public int Descent
    {
        get { return _Descent; }
        set
        {
            if (value == _Descent)
                return;
            _Descent = value;
        }
    }
    public int XMin
    {
        get { return _XMin; }
        set
        {
            if (value == _XMin)
                return;
            _XMin = value;
        }
    }
    public int YMin
    {
        get { return _YMin; }
        set
        {
            if (value == _YMin)
                return;
            _YMin = value;
        }
    }
    public int XMax
    {
        get { return _XMax; }
        set
        {
            if (value == _XMax)
                return;
            _XMax = value;
        }
    }
    public int YMax
    {
        get { return _YMax; }
        set
        {
            if (value == _YMax)
                return;
            _YMax = value;
        }
    }
}
