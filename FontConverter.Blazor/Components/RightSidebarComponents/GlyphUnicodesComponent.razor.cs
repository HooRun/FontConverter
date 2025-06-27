using FontConverter.Blazor.Interfaces;
using FontConverter.Blazor.ViewModels;
using Microsoft.AspNetCore.Components;
using Radzen;
using FontConverter.SharedLibrary.Models;

namespace FontConverter.Blazor.Components.RightSidebarComponents;

public partial class GlyphUnicodesComponent : ComponentBase, IRerenderable
{
    [Inject]
    private MainViewModel MainViewModel { get; set; } = default!;

    Variant variant = Variant.Outlined;
    bool floatFieldLabel = true;

    string? value;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        MainViewModel.RegisterComponent(nameof(GlyphUnicodesComponent), this);
    }

    public async Task ForceRender()
    {
        await InvokeAsync(StateHasChanged);
    }
}
