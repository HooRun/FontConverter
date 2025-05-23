using FontConverter.SharedLibrary.Models;

namespace FontConverter.Blazor.ViewModels;

public class FontContentsViewModel : FontContentViewModel
{
    public FontContentsViewModel()
    {
        _GlyphsCount = 0;
        _EmptyGlyphsCount = 0;
        _UnMappedGlyphsCount = 0;
        _SingleMappedGlyphsCount = 0;
        _MultiMappedGlyphsCount = 0;
        _UnicodesCount = 0;
    }

    private int _GlyphsCount;
    private int _EmptyGlyphsCount;
    private int _UnMappedGlyphsCount;
    private int _SingleMappedGlyphsCount;
    private int _MultiMappedGlyphsCount;
    private int _UnicodesCount;

    public int GlyphsCount
    {
        get { return _GlyphsCount; }
        set
        {
            if (value == _GlyphsCount)
                return;
            _GlyphsCount = value;
        }
    }
    public int EmptyGlyphsCount
    {
        get { return _EmptyGlyphsCount; }
        set
        {
            if (value == _EmptyGlyphsCount)
                return;
            _EmptyGlyphsCount = value;
        }
    }
    public int UnMappedGlyphsCount
    {
        get { return _UnMappedGlyphsCount; }
        set
        {
            if (value == _UnMappedGlyphsCount)
                return;
            _UnMappedGlyphsCount = value;
        }
    }
    public int SingleMappedGlyphsCount
    {
        get { return _SingleMappedGlyphsCount; }
        set
        {
            if (value == _SingleMappedGlyphsCount)
                return;
            _SingleMappedGlyphsCount = value;
        }
    }
    public int MultiMappedGlyphsCount
    {
        get { return _MultiMappedGlyphsCount; }
        set
        {
            if (value == _MultiMappedGlyphsCount)
                return;
            _MultiMappedGlyphsCount = value;
        }
    }
    public int UnicodesCount
    {
        get { return _UnicodesCount; }
        set
        {
            if (value == _UnicodesCount)
                return;
            _UnicodesCount = value;
        }
    }
}
