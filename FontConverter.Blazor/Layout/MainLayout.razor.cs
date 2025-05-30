using BlazorPro.BlazorSize;
using FontConverter.Blazor.Components.LeftSidebarComponents;
using FontConverter.Blazor.Interfaces;
using FontConverter.Blazor.ViewModels;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace FontConverter.Blazor.Layout;

public partial class MainLayout : LayoutComponentBase, IRerenderable
{
    [Inject]
    public NotificationService NotificationService { get; set; } = default!;

    [Inject]
    public DialogService DialogService { get; set; } = default!;

    [Inject]
    public MainViewModel MainViewModel { get; set; } = default!;

    protected override void OnInitialized()
    {
        base.OnInitialized(); 
        MainViewModel.RegisterComponent(nameof(MainLayout), this);
    }

    public void ForceRender()
    {
        StateHasChanged();
    }

    //protected override async Task OnInitializedAsync()
    //{
    //    ResizeListener.OnResized += ResizeHandler;
    //    var size = await ResizeListener.GetBrowserWindowSize();
    //    isMobile = size.Width <= 768;
    //}

    //void ResizeHandler(object? sender, BrowserWindowSize size)
    //{
    //    isMobile = size.Width <= 768;
    //    InvokeAsync(StateHasChanged);
    //}

    //public void Dispose()
    //{
    //    ResizeListener.OnResized -= ResizeHandler;
    //}

}
