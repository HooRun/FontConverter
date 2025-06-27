using FontConverter.Blazor.Interfaces;
using FontConverter.Blazor.ViewModels;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace FontConverter.Blazor.Components.RightSidebarComponents;

public partial class GlyphKerningsComponent : ComponentBase, IRerenderable
{
    [Inject]
    private MainViewModel MainViewModel { get; set; } = default!;

    Variant variant = Variant.Outlined;
    bool floatFieldLabel = true;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        MainViewModel.RegisterComponent(nameof(GlyphKerningsComponent), this);
    }

    public async Task ForceRender()
    {
        await InvokeAsync(StateHasChanged);
    }
}
