using BlazorPro.BlazorSize;
using FontConverter.Blazor.Components.LeftSidebarComponents;
using FontConverter.Blazor.Interfaces;
using FontConverter.Blazor.ViewModels;
using Microsoft.AspNetCore.Components;
using Radzen;
using FontConverter.SharedLibrary.Helpers;
using FontConverter.Blazor.Services;

namespace FontConverter.Blazor.Layout;

public partial class MainLayout : LayoutComponentBase, IRerenderable, IDisposable
{
    [Inject]
    public NotificationService NotificationService { get; set; } = default!;

    [Inject]
    public DialogService DialogService { get; set; } = default!;

    [Inject]
    public MainViewModel MainViewModel { get; set; } = default!;

    [Inject]
    public PredefinedDataService PredefinedData { get; set; } = default!;

    [Inject]
    private IResizeListener ResizeListener { get; set; } = default!;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        MainViewModel.RegisterComponent(nameof(MainLayout), this);
        ResizeListener.OnResized += OnWindowResized;
    }

    private void OnWindowResized(object? sender, BrowserWindowSize e)
    {
        MainViewModel.BrowserWindowWidth = e.Width;
    }

    public void Dispose()
    {
        ResizeListener.OnResized -= OnWindowResized;
    }

    public async Task ForceRender()
    {
        await InvokeAsync(StateHasChanged);
    }

}
