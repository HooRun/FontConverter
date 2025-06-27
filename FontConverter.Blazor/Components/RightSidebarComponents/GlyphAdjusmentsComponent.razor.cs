using FontConverter.Blazor.Interfaces;
using FontConverter.Blazor.Services;
using FontConverter.Blazor.ViewModels;
using Microsoft.AspNetCore.Components;
using Radzen;
using static FontConverter.SharedLibrary.Helpers.LVGLFontEnums;

namespace FontConverter.Blazor.Components.RightSidebarComponents;

public partial class GlyphAdjusmentsComponent : ComponentBase, IRerenderable
{
    [Inject]
    private MainViewModel MainViewModel { get; set; } = default!;

    [Inject]
    public PredefinedDataService PredefinedData { get; set; } = default!;

    Variant variant = Variant.Outlined;
    bool floatFieldLabel = true;

    private double _GammaValue = 1.0;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        MainViewModel.RegisterComponent(nameof(GlyphAdjusmentsComponent), this);
    }

    public async Task ForceRender()
    {
        await InvokeAsync(StateHasChanged);
    }

    private void OnAntiAliasChanged(bool newValue)
    {
        if (MainViewModel.LastSelectedGlyph != null && MainViewModel.LastSelectedGlyph.Adjusments.AntiAlias != newValue)
        {
            MainViewModel.LastSelectedGlyph.Adjusments.AntiAlias = newValue;
        }
    }

    private void OnDitherChanged(bool newValue)
    {
        if (MainViewModel.LastSelectedGlyph != null && MainViewModel.LastSelectedGlyph.Adjusments.Dither != newValue)
        {
            MainViewModel.LastSelectedGlyph.Adjusments.Dither = newValue;
        }
    }

    private void OnStyleChanged(object? newValue)
    {
        if (newValue != null && MainViewModel.LastSelectedGlyph != null && newValue is GLYPH_STYLE && MainViewModel.LastSelectedGlyph.Adjusments.Style != (GLYPH_STYLE)newValue)
        {
            MainViewModel.LastSelectedGlyph.Adjusments.Style = (GLYPH_STYLE)newValue;
        }
    }

    private void OnStrokeWidthChanged(int? newValue)
    {
        if (newValue != null && MainViewModel.LastSelectedGlyph != null && MainViewModel.LastSelectedGlyph.Adjusments.StrokeWidth != newValue)
        {
            MainViewModel.LastSelectedGlyph.Adjusments.StrokeWidth = (int)newValue;
        }
    }

    private void OnGammaChanged(int? newValue)
    {
        if (newValue != null && MainViewModel.LastSelectedGlyph != null && MainViewModel.LastSelectedGlyph.Adjusments.Gamma != newValue)
        {
            int gammaValue = Math.Clamp((int)newValue, 0, 100);
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
            MainViewModel.LastSelectedGlyph.Adjusments.Gamma = (int)newValue;
        }
    }

    private void OnThresholdChanged(int? newValue)
    {
        if (newValue != null && MainViewModel.LastSelectedGlyph != null && MainViewModel.LastSelectedGlyph.Adjusments.Threshold != newValue)
        {
            MainViewModel.LastSelectedGlyph.Adjusments.Threshold = (int)newValue;
        }
    }
}
