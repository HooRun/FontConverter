namespace FontConverter.Blazor.Models.GlyphsView;

public class GlyphsGroupedEntry
{
    public GlyphsGroupedEntry()
    {
        GroupItemsCount = 0;
        IsGroupHeader = false;
        GroupIcon = string.Empty;
        GroupHeader = string.Empty;
        GroupSubTitle = string.Empty;
        Items = [];
        ColumnsGap = 0;
        RowIndex = 0;
    }
    public int GroupItemsCount { get; set; }
    public bool IsGroupHeader { get; set; }
    public string GroupIcon { get; set; }
    public string GroupHeader { get; set; }
    public string GroupSubTitle { get; set; }
    public List<int> Items { get; set; }
    public int ColumnsGap { get; set; } 
    public int RowIndex { get; set; }
}
