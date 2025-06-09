using FontConverter.Blazor.Models.GlyphsView;
using Microsoft.AspNetCore.Components;

namespace FontConverter.Blazor.Components.GlyphsListViewComponents;

public partial class GlyphListRowComponent : ComponentBase
{
    [Parameter] 
    public EventCallback<(int GlyphID, bool IsSelected)> OnSelectionChanged { get; set; }

    [Parameter]
    public GlyphsGroupedEntry GroupedEntry { get; set; } = new();

}
