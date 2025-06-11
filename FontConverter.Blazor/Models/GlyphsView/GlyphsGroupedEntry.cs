namespace FontConverter.Blazor.Models.GlyphsView;

public class GlyphsGroupedEntry
{
    public GlyphsGroupedEntry()
    {
        GroupID = 0;
        GroupSelectedItemsCount = 0;
        GroupItemsCount = 0;
        IsGroupHeader = false;
        GroupIcon = string.Empty;
        GroupHeader = string.Empty;
        GroupSubTitle = string.Empty;
        Items = [];
        ColumnsGap = 0;
        RowIndex = 0;
    }
    public int GroupID { get; set; }
    public int GroupSelectedItemsCount { get; set; }
    public int GroupItemsCount { get; set; }
    public bool IsGroupHeader { get; set; }
    public string GroupIcon { get; set; }
    public string GroupHeader { get; set; }
    public string GroupSubTitle { get; set; }
    public List<int> Items { get; set; }
    public int ColumnsGap { get; set; } 
    public int RowIndex { get; set; }
}
