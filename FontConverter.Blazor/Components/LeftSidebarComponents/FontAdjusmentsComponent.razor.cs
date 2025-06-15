using FontConverter.Blazor.Interfaces;
using FontConverter.Blazor.Services;
using FontConverter.Blazor.ViewModels;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace FontConverter.Blazor.Components.LeftSidebarComponents;

public partial class FontAdjusmentsComponent : ComponentBase, IRerenderable
{
    [Inject]
    public PredefinedDataService PredefinedData { get; set; } = default!;

    [Inject]
    public MainViewModel MainViewModel { get; set; } = default!;

    int colLeft = 4;
    int colRight = 8;

    Variant variant = Variant.Outlined;
    bool floatFieldLabel = true;
    private double _GammaValue = 1.0;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        MainViewModel.RegisterComponent(nameof(FontAdjusmentsComponent), this);
    }

    public async Task ForceRender()
    {
        await InvokeAsync(StateHasChanged);
    }

    private void GammaChanged(int value)
    {
        int gammaValue = Math.Clamp(value, 0, 100);
        float gamma;
        if (gammaValue <= 50)
        {
            gamma = gammaValue / 50.0f;
        }
        else
        {
            gamma = 1.0f + ((gammaValue - 50) * 9.0f / 50.0f);
        }
        _GammaValue = gamma;
    }
}
