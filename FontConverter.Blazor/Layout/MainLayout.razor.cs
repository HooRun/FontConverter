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

    public async Task ForceRender()
    {
        await InvokeAsync(StateHasChanged);
    }

    private void OnGlyphSelection((int GhlyphID, bool IsSelected) args)
    {
        Console.WriteLine($"Glyph {args.GhlyphID} Selected: {args.IsSelected}");
    }

}
