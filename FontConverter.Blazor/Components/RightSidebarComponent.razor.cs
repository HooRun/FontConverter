using FontConverter.Blazor.Components.RightSidebarComponents;
using FontConverter.Blazor.Interfaces;
using FontConverter.Blazor.ViewModels;
using Microsoft.AspNetCore.Components;

namespace FontConverter.Blazor.Components;

public partial class RightSidebarComponent : ComponentBase, IRerenderable
{
    [Inject]
    private MainViewModel MainViewModel { get; set; } = default!;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        MainViewModel.RegisterComponent(nameof(RightSidebarComponent), this);
    }

    public async Task ForceRender()
    {
        await InvokeAsync(StateHasChanged);
        MainViewModel.RerenderMany(
            nameof(GlyphParametersComponent),
            nameof(GlyphUnicodesComponent),
            nameof(GlyphKerningsComponent),
            nameof(GlyphAdjusmentsComponent)
            );
    }
}
