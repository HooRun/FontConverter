using FontConverter.Blazor.Interfaces;
using FontConverter.Blazor.Layout;
using FontConverter.Blazor.ViewModels;
using Microsoft.AspNetCore.Components;
using Radzen;
using SkiaSharp;

namespace FontConverter.Blazor.Components;

public partial class LeftSidebarComponent : ComponentBase, IRerenderable
{
    [Inject] 
    NotificationService NotificationService { get; set; } = default!;

    [Inject]
    public MainViewModel MainViewModel { get; set; } = default!;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        MainViewModel.RegisterComponent(nameof(LeftSidebarComponent), this);
    }

    public void ForceRender()
    {
        StateHasChanged();
    }

}