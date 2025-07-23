using FontConverter.Blazor.Interfaces;
using FontConverter.Blazor.ViewModels;
using FontConverter.SharedLibrary.Helpers;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace FontConverter.Blazor.Components.RightSidebarComponents;

public partial class GlyphParametersComponent : ComponentBase, IRerenderable
{
    [Inject]
    private MainViewModel MainViewModel { get; set; } = default!;

    Variant variant = Variant.Outlined;
    bool floatFieldLabel = true;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        MainViewModel.RegisterComponent(nameof(GlyphParametersComponent), this);
    }

    public async Task ForceRender()
    {
        await InvokeAsync(StateHasChanged);
    }

    private void OnGlyphNameChanged(string? newValue)
    {
        if (newValue != null && MainViewModel.LastSelectedGlyph!=null && MainViewModel.LastSelectedGlyph.Name!=newValue)
        {
            MainViewModel.LastSelectedGlyph.Name = newValue;
            MainViewModel.OnGlyphPropertiesChanged?.Invoke(MainViewModel.LastSelectedGlyph.Index);
        }
    }

    private void OnGlyphDescriptionChanged(string? newValue)
    {
        if (newValue != null && MainViewModel.LastSelectedGlyph != null && MainViewModel.LastSelectedGlyph.Description != newValue)
        {
            MainViewModel.LastSelectedGlyph.Description = newValue;
            MainViewModel.OnGlyphPropertiesChanged?.Invoke(MainViewModel.LastSelectedGlyph.Index);
        }
    }

    private void OnGlyphWidthChanged(int? newValue)
    {
        if (newValue != null && MainViewModel.LastSelectedGlyph != null && MainViewModel.LastSelectedGlyph.Descriptor.Width != newValue)
        {
            MainViewModel.LastSelectedGlyph.Bitmap = RenderGlyphsToBitmapArrayHelper.ResizeBitmapWidth(
                MainViewModel.LastSelectedGlyph.OriginalBitmap,
                MainViewModel.LastSelectedGlyph.Descriptor.OriginalWidth,
                newValue ?? 0,
                MainViewModel.LastSelectedGlyph.Descriptor.Height,
                MainViewModel.FontSettingsViewModel.FontBitPerPixel
                );
            MainViewModel.LastSelectedGlyph.Descriptor.Width = newValue ?? 0;
            MainViewModel.OnGlyphPropertiesChanged?.Invoke(MainViewModel.LastSelectedGlyph.Index);
        }
    }

    private void OnGlyphHeightChanged(int? newValue)
    {
        if (newValue != null && MainViewModel.LastSelectedGlyph != null && MainViewModel.LastSelectedGlyph.Descriptor.Height != newValue)
        {
            MainViewModel.LastSelectedGlyph.Descriptor.Height = newValue ?? 0;
            MainViewModel.OnGlyphPropertiesChanged?.Invoke(MainViewModel.LastSelectedGlyph.Index);
        }
    }

    private void OnGlyphOffsetXChanged(int? newValue)
    {
        if (newValue != null && MainViewModel.LastSelectedGlyph != null && MainViewModel.LastSelectedGlyph.Descriptor.OffsetX != newValue)
        {
            MainViewModel.LastSelectedGlyph.Descriptor.OffsetX = newValue ?? 0;
            MainViewModel.OnGlyphPropertiesChanged?.Invoke(MainViewModel.LastSelectedGlyph.Index);
        }
    }

    private void OnGlyphOffsetYChanged(int? newValue)
    {
        if (newValue != null && MainViewModel.LastSelectedGlyph != null && MainViewModel.LastSelectedGlyph.Descriptor.OffsetY != newValue)
        {
            MainViewModel.LastSelectedGlyph.Descriptor.OffsetY = newValue ?? 0;
            MainViewModel.OnGlyphPropertiesChanged?.Invoke(MainViewModel.LastSelectedGlyph.Index);
        }
    }

    private void OnGlyphAdvanceWidthChanged(int? newValue)
    {
        if (newValue != null && MainViewModel.LastSelectedGlyph != null && MainViewModel.LastSelectedGlyph.Descriptor.AdvanceWidth != newValue)
        {
            MainViewModel.LastSelectedGlyph.Descriptor.AdvanceWidth = newValue ?? 0;
            MainViewModel.OnGlyphPropertiesChanged?.Invoke(MainViewModel.LastSelectedGlyph.Index);
        }
    }
}
