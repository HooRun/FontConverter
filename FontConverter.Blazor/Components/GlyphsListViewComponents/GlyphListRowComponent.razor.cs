using FontConverter.Blazor.EventsArgs;
using FontConverter.Blazor.Models.GlyphsView;
using FontConverter.Blazor.ViewModels;
using Microsoft.AspNetCore.Components;

namespace FontConverter.Blazor.Components.GlyphsListViewComponents;

public partial class GlyphListRowComponent : ComponentBase, IDisposable
{
    [Inject]
    public MainViewModel MainViewModel { get; set; } = default!;

    private GlyphsGroupedEntryModel _groupedEntry = new();

    [Parameter]
    public GlyphsGroupedEntryModel GroupedEntry
    {
        get => _groupedEntry;
        set
        {
            if (!ReferenceEquals(_groupedEntry, value))
            {
                _groupedEntry = value;
                _SelectedItemsCount = value.GroupSelectedItemsCount;
                _CheckBoxValue = _SelectedItemsCount switch
                {
                    0 => false,
                    var count when count == value.GroupItemsCount => true,
                    _ => null
                };
            }
        }
    }

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
        _CheckBoxValue = _SelectedItemsCount switch
        {
            0 => false,
            var count when count == GroupedEntry.GroupItemsCount => true,
            _ => null
        };
    }

    private void GroupSelectionChanges(bool? value)
    {
        if (value is null) value = true;

        if (_CheckBoxValue == value)
            return;

        _CheckBoxValue = value;

        MainViewModel.GroupSelectionChanged(GroupedEntry.GroupID, value == true);
    }

    private void SelectionChanged(GroupSelectionChangedEventArgs selectionInfo)
    {
        foreach (var info in selectionInfo.GroupsList)
        {
            if (info.GroupID == GroupedEntry.GroupID)
            {
                if (GroupedEntry.GroupSelectedItemsCount != info.SelectedItemsCount)
                {
                    GroupedEntry.GroupSelectedItemsCount = info.SelectedItemsCount;
                    _SelectedItemsCount = info.SelectedItemsCount;
                    _CheckBoxValue = _SelectedItemsCount switch
                    {
                        0 => false,
                        var count when count == GroupedEntry.GroupItemsCount => true,
                        _ => null
                    };

                    InvokeAsync(StateHasChanged);
                }
                break;
            }
        }
    }


    public void Dispose()
    {
        MainViewModel.OnGroupSelectionChanged -= SelectionChanged;
    }
}
