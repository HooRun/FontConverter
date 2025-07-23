using FontConverter.Blazor.Interfaces;
using FontConverter.Blazor.Models;
using FontConverter.Blazor.Models.GlyphsView;
using FontConverter.Blazor.Services;
using FontConverter.Blazor.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;

namespace FontConverter.Blazor.Components.GlyphsListViewComponents;

public partial class GlyphListComponent : ComponentBase, IRerenderable, IAsyncDisposable, IDisposable
{
    [Inject]
    public MainViewModel MainViewModel { get; set; } = default!;

    [Inject]
    public IJSRuntime JSRuntime { get; set; } = default!;

    private Virtualize<GlyphsGroupedEntryModel>? virtualizeComponent;

    private int _ListHorizontalGap = 10;
    private int _ListVerticalGap = 0;
    private int defaultVerticalGap = 4;
    private int glyphHeaderSize = 20;
    private int GlyphItemHeight = 0;
    private int GlyphItemWidth = 0;
    private int CountOfColumns = 1;
    private float _VirtualizeRowHeight = 75.0f;
    private int _VirtualizeRowCounts = 0;

    private string lastLoadDataArgs = "";
    private Task lastLoadDataTask = Task.CompletedTask;
    private List<GlyphsGroupedEntryModel> _VirtualizeDataItems = [];

    private DotNetObjectReference<GlyphListComponent>? _ObjRef;
    private CancellationTokenSource? _ResizeDebounceCts;
    private CancellationTokenSource? _VirtualizeDebounceCts;
    private ElementDimensionsModel itemsContainerDimensions = new();

    private int _VirtualizeKey = 0;
    private bool _Dispose = false;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        MainViewModel.RegisterComponent(nameof(GlyphListComponent), this);
        _Dispose = false;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _ObjRef = DotNetObjectReference.Create(this);
            await JSRuntime.InvokeVoidAsync("elementResizeObserver.observe", "glyph-list-container", _ObjRef,"OnElementResized");
        }
    }

    public async Task ForceRender()
    {
        _VirtualizeKey++;
        //await Task.Delay(100);
        if (virtualizeComponent is not null)
        {
            PrecalculateVirtualizeInfo();
            await virtualizeComponent.RefreshDataAsync();
        }
        await InvokeAsync(StateHasChanged);
    }

    [JSInvokable]
    public async Task OnElementResized(ElementDimensionsModel size)
    {
        _ResizeDebounceCts?.Cancel();
        _ResizeDebounceCts = new CancellationTokenSource();
        try
        {
            await Task.Delay(100, _ResizeDebounceCts.Token);
            itemsContainerDimensions = size;
            if (virtualizeComponent is not null)
            {
                PrecalculateVirtualizeInfo();
                await virtualizeComponent.RefreshDataAsync();
            }
            await InvokeAsync(StateHasChanged);

        }
        catch (TaskCanceledException) { }
    }

    private void UpdateCountOfRowsAndColumns()
    {
        GlyphItemWidth = MainViewModel.GlyphViewItemPropertiesViewModel.Width;
        GlyphItemHeight = MainViewModel.GlyphViewItemPropertiesViewModel.Height;
        if (GlyphItemWidth <= 0)
        {
            CountOfColumns = 1;
            _ListVerticalGap = 0;
            return;
        }
        (int columns, int gap) = CalculateColumnsAndGap(itemsContainerDimensions.Width, GlyphItemWidth);
        CountOfColumns = columns;
        _ListVerticalGap = gap;
    }

    private (int columns, int gap) CalculateColumnsAndGap(double containerWidth, int itemWidth)
    {
        int maxColumns = Math.Max(1, (int)(containerWidth / itemWidth));
        int gap = 0;
        for (int c = maxColumns; c >= 1; c--)
        {
            gap = (int)((containerWidth - c * itemWidth) / (c - 1.0));
            if (c == 1 || gap >= defaultVerticalGap)
                return (c, gap);
        }
        return (1, 0);
    }

    // Thanks Radzen
    private Task InvokeLoadDataAsync(int startIndex, int count)
    {
        int endIndex = startIndex + count;
        int currentIndex = 0;
        _VirtualizeDataItems = [];

        for (int groupIndex = 0; groupIndex < MainViewModel.GlyphsGroupedList.Count; groupIndex++)
        {
            var group = MainViewModel.GlyphsGroupedList[groupIndex];

            int rowCount = 0;
            if (group.IsExpanded && group.IsLoaded && group.Items is { Count: > 0 })
            {
                rowCount = (int)Math.Ceiling(group.Items.Count / (double)CountOfColumns);
            }

            int groupTotalRows = 1 + rowCount;

            if (currentIndex + groupTotalRows <= startIndex)
            {
                currentIndex += groupTotalRows;
                continue;
            }

            if (currentIndex >= endIndex)
                break;

            if (currentIndex >= startIndex && currentIndex < endIndex)
            {
                _VirtualizeDataItems.Add(new GlyphsGroupedEntryModel
                {
                    GroupID = groupIndex,
                    IsGroupHeader = true,
                    GroupHeader = group.Header,
                    GroupSubTitle = group.SubTitle,
                    GroupIcon = group.Icon,
                    GroupItemsCount = group.Items?.Count ?? 0,
                    GroupSelectedItemsCount = group.SelectedItems.Count,
                    Items = Enumerable.Repeat(-1, CountOfColumns).ToList(),
                    ColumnsGap = _ListVerticalGap,
                    RowIndex = currentIndex - groupIndex,
                });
            }
            currentIndex++;

            if (group.IsExpanded && group.IsLoaded && group.Items is { Count: > 0 })
            {
                for (int i = 0; i < rowCount; i++)
                {
                    if (currentIndex >= endIndex)
                        break;

                    if (currentIndex >= startIndex)
                    {
                        var chunk = group.Items.Skip(i * CountOfColumns).Take(CountOfColumns).ToList();

                        if (chunk.Count < CountOfColumns)
                            chunk.AddRange(Enumerable.Repeat(-1, CountOfColumns - chunk.Count));

                        _VirtualizeDataItems.Add(new GlyphsGroupedEntryModel
                        {
                            GroupID = groupIndex,
                            IsGroupHeader = false,
                            GroupHeader = group.Header,
                            GroupSubTitle = group.SubTitle,
                            GroupIcon = group.Icon,
                            GroupItemsCount = group.Items.Count,
                            GroupSelectedItemsCount = group.SelectedItems.Count,
                            Items = chunk,
                            ColumnsGap = _ListVerticalGap,
                            RowIndex = currentIndex - groupIndex,
                        });
                    }
                    currentIndex++;
                }
            }
        }

        return Task.CompletedTask;
    }

    private async ValueTask<ItemsProviderResult<GlyphsGroupedEntryModel>> LoadChunkedGlyphs(ItemsProviderRequest request)
    {
        string loadDataArgs = $"{request.StartIndex}|{request.Count}|{CountOfColumns}|{_ListVerticalGap}|{_VirtualizeRowCounts}";

        if (loadDataArgs != lastLoadDataArgs)
        {
            lastLoadDataArgs = loadDataArgs;
            lastLoadDataTask = InvokeLoadDataAsync(request.StartIndex, request.Count);
        }

        await lastLoadDataTask;

        return new ItemsProviderResult<GlyphsGroupedEntryModel>(_VirtualizeDataItems, _VirtualizeRowCounts);
    }


    private void PrecalculateVirtualizeInfo()
    {
        UpdateCountOfRowsAndColumns();
        UpdateVirtualizeRowCount();
        UpdateVirtualizeRowHeight();
    }

    private void UpdateVirtualizeRowCount()
    {
        _VirtualizeRowCounts = 0;
        foreach (var group in MainViewModel.GlyphsGroupedList)
        {
            _VirtualizeRowCounts++;

            if (group.IsExpanded && group.IsLoaded && group.Items != null && CountOfColumns > 0)
            {
                int rowCount = (int)Math.Ceiling((double)group.Items.Count / CountOfColumns);
                _VirtualizeRowCounts += rowCount;
            }
        }
    }

    private void UpdateVirtualizeRowHeight()
    {
        _VirtualizeRowHeight = ((((_VirtualizeRowCounts - MainViewModel.GlyphsGroupedList.Count) * MainViewModel.GlyphViewItemPropertiesViewModel.Height) + (MainViewModel.GlyphsGroupedList.Count * 75)) / (float)_VirtualizeRowCounts);
    }

    public async ValueTask DisposeAsync()
    {
        if (!_Dispose)
        {
            if (_ObjRef != null)
            {
                await JSRuntime.InvokeVoidAsync("elementResizeObserver.unobserve", "glyph-list-container");
            }
            Dispose();
        }
    }

    public void Dispose()
    {
        if (!_Dispose)
        {
            _Dispose = true;
            _ObjRef?.Dispose();
            _ResizeDebounceCts?.Dispose();
        }
    }
}
