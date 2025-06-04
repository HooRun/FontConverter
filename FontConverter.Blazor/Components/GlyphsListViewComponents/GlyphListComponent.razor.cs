using BlazorPro.BlazorSize;
using FontConverter.Blazor.Components.LeftSidebarComponents;
using FontConverter.Blazor.Interfaces;
using FontConverter.Blazor.Models;
using FontConverter.Blazor.ViewModels;
using FontConverter.SharedLibrary.Models;
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

    [Parameter] public EventCallback<(int GlyphID, bool IsSelected)> OnSelectionChanged { get; set; }

    private Virtualize<LVGLGlyph[]>? virtualizeComponent;

    private int _ListHorizontalGap = 10;
    private int _ListVerticalGap = 0;
    private int defaultVerticalGap = 4;
    private int glyphHeaderSize = 20;
    private int GlyphItemHeight = 0;
    private int GlyphItemWidth = 0;
    private int CountOfColumns = 1;

    private DotNetObjectReference<GlyphListComponent>? _ObjRef;
    private CancellationTokenSource? debounceCts;
    private ElementDimensions itemsContainerDimensions = new();

    private int _VirtualizeComponentKey = 0;
    private int _VirtualizeItemsCount = 0;

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
        _VirtualizeComponentKey++;
        await Task.Delay(100);
        if (virtualizeComponent is not null)
        {
            await virtualizeComponent.RefreshDataAsync();
        }
        await InvokeAsync(StateHasChanged);
    }

    [JSInvokable]
    public async Task OnElementResized(ElementDimensions size)
    {
        debounceCts?.Cancel();
        debounceCts = new CancellationTokenSource();
        try
        {
            await Task.Delay(100, debounceCts.Token);
            itemsContainerDimensions = size;
            if (virtualizeComponent is not null)
            {
                await virtualizeComponent.RefreshDataAsync();
            }
            await InvokeAsync(StateHasChanged);

        }
        catch (TaskCanceledException) { }
    }

    private void UpdateCountOfRowsAndColumns()
    {
        GlyphItemWidth = MainViewModel.GlyphItemWidth;
        GlyphItemHeight = MainViewModel.GlyphItemHeight;
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

    private async ValueTask<ItemsProviderResult<LVGLGlyph[]>> LoadChunkedGlyphs(ItemsProviderRequest request)
    {
        UpdateCountOfRowsAndColumns();

        int startIndex = request.StartIndex * CountOfColumns;
        int count = request.Count * CountOfColumns;
        int totalRowCount = (int)Math.Ceiling((double)MainViewModel.TotalGlyphsCount / CountOfColumns);
        int endIndex = Math.Min(startIndex + count, MainViewModel.TotalGlyphsCount);

        var flatGlyphs = await MainViewModel.GetGlyphsAsync(startIndex, endIndex - startIndex);

        var rows = new List<LVGLGlyph[]>();
        for (int i = 0; i < flatGlyphs.Count; i += CountOfColumns)
            rows.Add(flatGlyphs.Skip(i).Take(CountOfColumns).ToArray());

        return new ItemsProviderResult<LVGLGlyph[]>(rows, totalRowCount);
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
            debounceCts?.Dispose();
        }
    }
}
