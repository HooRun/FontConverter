namespace FontConverter.Blazor.ViewModels;

public class GlyphViewItemPropertiesViewModel : BaseViewModel
{
    public GlyphViewItemPropertiesViewModel()
    {
        _XMin = 0;
        _BaseLine = 0;
        _ItemWidth = 0;
        _ItemHeight = 0;
        _Zoom = 1;
        ItemPadding = 10;
    }

    private int _XMin;
    private int _BaseLine;
    private int _ItemWidth;
    private int _ItemHeight;
    private int _Zoom;

    public int HeaderHeight => 20;
    public int BorderWidth => 2;
    public int CanvasPadding => 5;

    public int ItemPadding { get; private set; }
    
    public int XMin
    {
        get { return _XMin; }
        set { SetProperty(ref _XMin, value); }
    }
    public int BaseLine
    {
        get { return _BaseLine; }
        set { SetProperty(ref _BaseLine, value); }
    }
    public int ItemWidth
    {
        get { return _ItemWidth; }
        set { SetProperty(ref _ItemWidth, value); }
    }
    public int ItemHeight
    {
        get { return _ItemHeight; }
        set { SetProperty(ref _ItemHeight, value); }
    }
    public int Zoom
    {
        get { return _Zoom; }
        set { SetProperty(ref _Zoom, value); }
    }

    public int Width => (ItemWidth * Zoom) + (CanvasPadding * 2) + (BorderWidth * 2);
    public int Height => (ItemHeight * Zoom) + (CanvasPadding * 2) + HeaderHeight + (BorderWidth * 2);
    public int CanvasWidth => Width - (BorderWidth * 2);
    public int CanvasHeight => Height - HeaderHeight - (BorderWidth * 2);
    public int XAxis => CanvasHeight - (BaseLine * Zoom) - CanvasPadding;
    public int YAxis => XMin > 0 ? CanvasPadding : (-XMin * _Zoom) + CanvasPadding;

}
