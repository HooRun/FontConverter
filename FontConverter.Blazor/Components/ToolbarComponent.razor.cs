using FontConverter.Blazor.Components.LeftSidebarComponents;
using FontConverter.Blazor.Interfaces;
using FontConverter.Blazor.ViewModels;
using Microsoft.AspNetCore.Components;

namespace FontConverter.Blazor.Components;

public partial class ToolbarComponent : ComponentBase, IRerenderable
{
    [Inject]
    public MainViewModel MainViewModel { get; set; } = default!;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        MainViewModel.RegisterComponent(nameof(ToolbarComponent), this);
    }

    public void ForceRender()
    {
        StateHasChanged();
    }
}
