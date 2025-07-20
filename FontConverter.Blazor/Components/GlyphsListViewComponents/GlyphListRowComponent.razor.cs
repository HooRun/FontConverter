using FontConverter.Blazor.EventsArgs;
using FontConverter.Blazor.Models.GlyphsView;
using FontConverter.Blazor.ViewModels;
using Microsoft.AspNetCore.Components;

namespace FontConverter.Blazor.Components.GlyphsListViewComponents;

public partial class GlyphListRowComponent : ComponentBase, IDisposable
{
    [Inject]
    public MainViewModel MainViewModel { get; set; } = default!;

    [Parameter]
    public GlyphsGroupedEntryModel GroupedEntry { get; set; } = new();

    private bool? _CheckBoxValue = false;
    private int _SelectedItemsCount = 0;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        MainViewModel.OnGroupSelectionChanged += SelectionChanged;
    }

    protected override async Task OnParametersSetAsync()
    {
        _SelectedItemsCount = GroupedEntry.GroupSelectedItemsCount;
        if (_SelectedItemsCount == 0)
        {
            _CheckBoxValue = false;
        }
        else if (_SelectedItemsCount == GroupedEntry.GroupItemsCount)
        {
            _CheckBoxValue = true;
        }
        else
        {
            _CheckBoxValue = null;
        }
        await InvokeAsync(StateHasChanged);
    }

    private void GroupSelectionChanges(bool? value)
    {
        if (value is null) value = true;
        _CheckBoxValue = value;
        if (value == true)
        {
            MainViewModel.GroupSelectionChanged(GroupedEntry.GroupID, true);
        }
        else if (value == false)
        {
            MainViewModel.GroupSelectionChanged(GroupedEntry.GroupID, false);
        }
    }

    private void SelectionChanged(GroupSelectionChangedEventArgs selectionInfo)
    {
        foreach (var info in selectionInfo.GroupsList)
        {
            if (info.GroupID == GroupedEntry.GroupID)
            {
                GroupedEntry.GroupSelectedItemsCount = info.SelectedItemsCount;
                _SelectedItemsCount = GroupedEntry.GroupSelectedItemsCount;
                if (_SelectedItemsCount == 0)
                {
                    _CheckBoxValue = false;
                }
                else if (_SelectedItemsCount == GroupedEntry.GroupItemsCount)
                {
                    _CheckBoxValue = true;
                }
                else
                {
                    _CheckBoxValue = null;
                }
            }
        }
        StateHasChanged();
    }

    public void Dispose()
    {
        MainViewModel.OnGroupSelectionChanged -= SelectionChanged;
    }
}
