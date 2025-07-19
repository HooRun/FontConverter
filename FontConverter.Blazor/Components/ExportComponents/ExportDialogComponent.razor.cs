using FontConverter.Blazor.Components.PrismJSComponents;
using FontConverter.Blazor.Interfaces;
using FontConverter.Blazor.Services;
using FontConverter.Blazor.ViewModels;
using FontConverter.SharedLibrary.Models;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;

namespace FontConverter.Blazor.Components.ExportComponents;

public partial class ExportDialogComponent : ComponentBase, IRerenderable
{
    [Inject]
    MainViewModel MainViewModel { get; set; } = default!;

    [Inject]
    PredefinedDataService PredefinedData { get; set; } = default!;

    private HashSet<LVGLGlyph> _SelectedGlyphsSet = new();
    private IList<LVGLGlyph> _SelectedGlyphsList => _SelectedGlyphsSet.ToList();

    private int _SelectedTabIndex = 0;

    RadzenDataGrid<LVGLGlyph>? _GlyphsDataGrid;

    IList<LVGLGlyph> GridData { get; set; } = [];

    private int _GridGroupOption = 1;

    private bool _GridAllowVirtualization = true;
    private bool _GridllowPaging = false;
    private bool? _GridAllGroupsExpanded = false;

    private int _TotalGlyphCount = 0;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        GridData = MainViewModel.LVGLFont.Glyphs.Values.ToList();
        MainViewModel.RegisterComponent(nameof(CodeBlockComponent), this);
        _AllUnicodesFilter = MainViewModel.GlyphsList.Values
            .Where(g => g.CodePoints.Count > 0)
            .SelectMany(g => g.CodePoints.Values)
            .Select(c => c.CodePointString)
            .Distinct()
            .OrderBy(s => s)
            .ToList();
        if (!_AllUnicodesFilter.Contains(string.Empty)) 
            _AllUnicodesFilter.Insert(0, string.Empty);
    }

    public async Task ForceRender()
    {
        await InvokeAsync(StateHasChanged);
    }

    private void OnGlyphsChanged(IList<LVGLGlyph>? newValue)
    {
        _SelectedGlyphsSet = newValue != null
            ? new HashSet<LVGLGlyph>(newValue)
            : new HashSet<LVGLGlyph>();
    }

    IList<string> _AllUnicodesFilter = [];
    IList<string>? _SelecedUnicodesFirstFilter;

    private void OnSelecedUnicodesFirstFilterChange(object value)
    {
        if (_SelecedUnicodesFirstFilter != null && !_SelecedUnicodesFirstFilter.Any())
        {
            _SelecedUnicodesFirstFilter = null;
        }

    }

    private void FilterCleared()
    {
        _SelecedUnicodesFirstFilter = null;
        
    }

    private async Task OnSelectAllChangedAsync(object? isChecked)
    {
        await Task.Yield();

        if (isChecked is bool checkedValue)
        {
            if (checkedValue)
            {
                _SelectedGlyphsSet = GridData.ToHashSet();
            }
            else
            {
                _SelectedGlyphsSet.Clear();
            }
        }

        await InvokeAsync(StateHasChanged);
    }

    private async Task OnGroupCheckboxChanged(string? groupKey, bool? checkedValue)
    {
        await Task.Run(() =>
        {
            List<LVGLGlyph> groupItems = [];

            if (_GridGroupOption == 2)
            {
                groupItems = GridData
                .Where(g => g.GlyphGroupByContentHeader?.Equals(groupKey) == true)
                .ToList();
            }
            else if (_GridGroupOption == 3)
            {
                groupItems = GridData
                 .Where(g => g.GlyphGroupByUnicodeRangeHeader?.Equals(groupKey) == true)
                 .ToList();
            }
            
            if (checkedValue is bool isChecked)
            {
                if (isChecked)
                {
                    _SelectedGlyphsSet.UnionWith(groupItems);
                }
                else
                {
                    _SelectedGlyphsSet.ExceptWith(groupItems);
                }
            }
        });

        await InvokeAsync(StateHasChanged);
    }

    private async Task OnRowCheckboxChangedAsync(LVGLGlyph item, object isChecked)
    {
        if (isChecked is bool checkedValue)
        {
            if (checkedValue)
                _SelectedGlyphsSet.Add(item);
            else
                _SelectedGlyphsSet.Remove(item);
        }

        await InvokeAsync(StateHasChanged);
    }

    private bool? GetSelectAllValue()
    {
        if (_SelectedGlyphsSet.Count == 0)
            return false;

        if (_SelectedGlyphsSet.Count < GridData.Count)
            return null;

        return true;
    }

    private bool? GetGroupCheckBoxValue(string? groupKey)
    {
        List<LVGLGlyph> groupItems = [];

        if (_GridGroupOption == 2)
        {
            groupItems = GridData.Where(g => g.GlyphGroupByContentHeader?.Equals(groupKey) == true).ToList();
        }
        else if (_GridGroupOption == 3)
        {
            groupItems = GridData.Where(g => g.GlyphGroupByUnicodeRangeHeader?.Equals(groupKey) == true).ToList();
        }

        if (groupItems.Count == 0)
            return false;

        var selectedCount = groupItems.Count(g => _SelectedGlyphsSet.Contains(g));

        if (selectedCount == 0)
            return false;
        else if (selectedCount == groupItems.Count)
            return true;
        else
            return null;
    }

    private async Task OnGridGroupOptionChange(int value)
    {
        _GridGroupOption = value;
        if (_GlyphsDataGrid != null)
        {
            if (_GridGroupOption==1)
            {
                GridData = MainViewModel.LVGLFont.Glyphs.Values.ToList();
                _GlyphsDataGrid.Groups.Clear();
                _GridAllowVirtualization = true;
                _GridllowPaging = false;
            }
            else if (_GridGroupOption == 2)
            {
                GridData = await Task.Run(GroupByGlyphContent);
                _GlyphsDataGrid.Groups.Clear();
                _GlyphsDataGrid.Groups.Add(new GroupDescriptor() { Property = "GlyphGroupByContentHeader", SortOrder = SortOrder.Ascending });
                _GridAllowVirtualization = true;
                _GridllowPaging = false;
            }
            else if (_GridGroupOption == 3)
            {
                GridData = await Task.Run(()=> GroupByUnicodeRanges(PredefinedData.UnicodeBlockCollection.Blocks));
                _GlyphsDataGrid.Groups.Clear();
                _GlyphsDataGrid.Groups.Add(new GroupDescriptor() { Property = "GlyphGroupByUnicodeRangeHeader", SortOrder = SortOrder.Ascending });
                _GridAllowVirtualization = true;
                _GridllowPaging = false;
            }
            _GridAllGroupsExpanded = false;
            _TotalGlyphCount = GridData.Count;
            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task<IList<LVGLGlyph>> GroupByGlyphContent()
    {
        await Task.Yield();

        IList<LVGLGlyph> groupedByContentList = [];

        foreach (var glyph in MainViewModel.GlyphsList.Values)
        {
            LVGLGlyph newEmptyGlyph = new(glyph);
            if (glyph.IsEmpty)
            {
                newEmptyGlyph.GlyphGroupByContentHeader = "1";
            }
            else
            {
                newEmptyGlyph.GlyphGroupByContentHeader = "2";
            }

            LVGLGlyph newMappedGlyph = new(glyph);
            if (glyph.IsUnMapped)
            {
                newMappedGlyph.GlyphGroupByContentHeader = "3";
            }
            else if (glyph.IsSingleMapped)
            {
                newMappedGlyph.GlyphGroupByContentHeader = "4";
            }
            else if (glyph.IsMultiMapped)
            {
                newMappedGlyph.GlyphGroupByContentHeader = "5";
            }

            groupedByContentList.Add(newEmptyGlyph);
            groupedByContentList.Add(newMappedGlyph);
        }

        return groupedByContentList.OrderBy(g => g.Index).ToList();
    }

    private async Task<IList<LVGLGlyph>> GroupByUnicodeRanges(SortedDictionary<uint, UnicodeBlock> blocks)
    {
        await Task.Yield();

        IList<LVGLGlyph> groupedByContentList = [];

        foreach (var glyph in MainViewModel.GlyphsList.Values)
        {
            
            if (glyph.CodePoints.Count<=0)
            {
                LVGLGlyph newRangesGlyphs = new(glyph);
                newRangesGlyphs.GlyphGroupByUnicodeRangeHeader = "( Unmapped Glyps )";
                groupedByContentList.Add(newRangesGlyphs);
            }
            else
            { 
                foreach (var cp in glyph.CodePoints.Values)
                {
                    LVGLGlyph newRangesGlyphs = new(glyph);
                    newRangesGlyphs.GlyphGroupByUnicodeRangeHeader = $"({blocks[cp.Block].StartString}-{blocks[cp.Block].EndString}) {blocks[cp.Block].Name}";
                    groupedByContentList.Add(newRangesGlyphs);
                }
            }
        }

        return groupedByContentList.OrderBy(g => g.Index).ToList();
    }

    private string GetGroupLabel(string? groupKey)
    {
        if (_GridGroupOption == 2)
        {
            if (groupKey == "1")
            {
                return "Empty Glyphs";
            }
            else if (groupKey == "2")
            {
                return "Non-Empty Glyphs";
            }
            else if (groupKey == "3")
            {
                return "Unmapped Glyphs";
            }
            else if (groupKey == "4")
            {
                return "Single Mapped Glyphs";
            }
            else if (groupKey == "5")
            {
                return "Multi Mapped Glyphs";
            }
        }
        else if (_GridGroupOption == 3)
        {
            return groupKey ?? string.Empty;
        }
        return string.Empty;
    }

    

}
