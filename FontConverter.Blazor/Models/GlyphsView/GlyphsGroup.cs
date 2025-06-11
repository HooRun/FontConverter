using FontConverter.SharedLibrary.Models;

namespace FontConverter.Blazor.Models.GlyphsView;

public class GlyphsGroup
{
    public GlyphsGroup()
    {
        Icon = string.Empty;
        Header = string.Empty;
        SubTitle = string.Empty;
        LoadItemsAsync = null;
        Items = [];
        SelectedItems = [];
        IsExpanded = false;
        IsLoaded = false;
    }

    public string Icon { get; set; }
    public string Header { get; set; }
    public string SubTitle { get; set; }
    public Func<Task<List<int>>>? LoadItemsAsync { get; set; }
    public List<int> Items { get; set; }
    public List<int> SelectedItems { get; set; }
    public bool IsExpanded { get; set; }
    public bool IsLoaded { get; set; }
}
