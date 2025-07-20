using FontConverter.Blazor.Interfaces;
using FontConverter.Blazor.ViewModels;
using Microsoft.AspNetCore.Components;

namespace FontConverter.Blazor.Components;

public partial class LeftSidebarComponent : ComponentBase, IRerenderable
{
    [Inject]
    public MainViewModel MainViewModel { get; set; } = default!;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        MainViewModel.RegisterComponent(nameof(LeftSidebarComponent), this);
    }

    public async Task ForceRender()
    {
        await InvokeAsync(StateHasChanged);
    }

}