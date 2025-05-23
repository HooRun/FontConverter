using FontConverter.SharedLibrary.Models;

namespace FontConverter.Blazor.ViewModels;

public class FontContentViewModel
{
    public FontContentViewModel()
    {
        _Header = string.Empty;
        _Icon = string.Empty;
        _Count = 0;
        _IsSelected = false;
        _Contents = new();
    }

    private string _Header;
    private string _Icon;
    private int _Count;
    private bool _IsSelected;
    private SortedList<string, FontContentViewModel> _Contents;

    public string Header
    {
        get { return _Header; }
        set
        {
            if (value == _Header)
                return;
            _Header = value;
        }
    }
    public string Icon
    {
        get { return _Icon; }
        set
        {
            if (value == _Icon)
                return;
            _Icon = value;
        }
    }
    public int Count
    {
        get { return _Count; }
        set
        {
            if (value == _Count)
                return;
            _Count = value;
        }
    }
    public bool IsSelected
    {
        get { return _IsSelected; }
        set
        {
            if (value == _IsSelected)
                return;
            _IsSelected = value;
        }
    }
    public SortedList<string, FontContentViewModel> Contents
    {
        get { return _Contents; }
        set
        {
            if (value == _Contents)
                return;
            _Contents = value;
        }
    }

    public IEnumerable<FontContentViewModel> Children => _Contents?.Values ?? Enumerable.Empty<FontContentViewModel>();
}
