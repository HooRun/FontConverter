using FontConverter.SharedLibrary.Models;

namespace FontConverter.Blazor.ViewModels;

public class FontContentViewModel : BaseViewModel
{
    public FontContentViewModel()
    {
        _Header = string.Empty;
        _SubTitle = string.Empty;
        _Icon = string.Empty;
        _Count = 0;
        _IsSelected = false;
        _Items = [];
        _Contents = new();
    }

    private string _Header;
    private string _SubTitle;
    private string _Icon;
    private int _Count;
    private bool _IsSelected;
    private List<int> _Items;
    private SortedList<string, FontContentViewModel> _Contents;

    public string Header
    {
        get { return _Header; }
        set { SetProperty(ref _Header, value); }
    }
    public string SubTitle
    {
        get { return _SubTitle; }
        set { SetProperty(ref _SubTitle, value); }
    }
    public string Icon
    {
        get { return _Icon; }
        set { SetProperty(ref _Icon, value); }
    }
    public int Count
    {
        get { return _Count; }
        set { SetProperty(ref _Count, value); }
    }
    public bool IsSelected
    {
        get { return _IsSelected; }
        set { SetProperty(ref _IsSelected, value); }
    }
    public List<int> Items
    {
        get { return _Items; }
        set { SetProperty(ref _Items, value); }
    }
    public SortedList<string, FontContentViewModel> Contents
    {
        get { return _Contents; }
        set { SetProperty(ref _Contents, value); }
    }

    public IEnumerable<FontContentViewModel> Children => _Contents?.Values ?? Enumerable.Empty<FontContentViewModel>();
}
