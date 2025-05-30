using FontConverter.Blazor.Components.LeftSidebarComponents;
using FontConverter.Blazor.Interfaces;
using FontConverter.Blazor.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FontConverter.Blazor.Components.GlyphsListViewComponents;

public partial class GlyphViewListComponent : ComponentBase, IRerenderable
{
    [Inject]
    public MainViewModel MainViewModel { get; set; } = default!;

    [Inject]
    public IJSRuntime JS { get; set; } = default!;

    [Parameter]
    public EventCallback<(int GlyphID, bool IsSelected)> OnSelectionChanged { get; set; }

    [Parameter]
    public string Style { get; set; } = string.Empty;

    private bool isLoading = true;
    private int listHeight;
    private DotNetObjectReference<GlyphViewListComponent>? _dotNetRef;

    private int GlyphItemHeight => (MainViewModel.GlyphViewItemPropertiesViewModel.ItemHeight * MainViewModel.GlyphViewItemPropertiesViewModel.Zoom) + MainViewModel.GlyphViewItemPropertiesViewModel.ItemPadding + 20;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        MainViewModel.RegisterComponent(nameof(GlyphViewListComponent), this);
        if (MainViewModel.LVGLFont.Glyphs.Count>0) isLoading = false;
    }

    public void ForceRender()
    {
        if (MainViewModel.LVGLFont.Glyphs.Count > 0) isLoading = false;
        StateHasChanged();
    }

    //protected override async Task OnAfterRenderAsync(bool firstRender)
    //{
    //    if (firstRender)
    //    {

    //        //await JS.InvokeVoidAsync("observeParentResize", "glyph_list_container",
    //        //    DotNetObjectReference.Create(this), nameof(OnParentResized));

    //        //await JS.InvokeVoidAsync("observeSelfResize", "glyph_list_container",
    //        //    DotNetObjectReference.Create(this), nameof(OnSelfResized));

    //        //// await JS.InvokeVoidAsync("unobserveResize", "glyph_list_container"); // Stop observing resize events
    //    }
    //}

    //[JSInvokable]
    //public async Task OnParentResized(double width, double height)
    //{
    //    await Task.Delay(1);
    //    Console.WriteLine($"Parent Size: {width} x {height}");
    //}

    //[JSInvokable]
    //public async Task OnSelfResized(double width, double height)
    //{
    //    Console.WriteLine($"Self Size: {width} x {height}");
    //    listHeight = (int)(height);
    //    await InvokeAsync(StateHasChanged);
    //}

    //public void Dispose()
    //{
    //    _dotNetRef?.Dispose();
    //}


    

}
