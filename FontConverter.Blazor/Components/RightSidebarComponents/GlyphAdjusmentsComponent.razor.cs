using FontConverter.Blazor.Interfaces;
using FontConverter.Blazor.Services;
using FontConverter.Blazor.ViewModels;
using FontConverter.SharedLibrary.Helpers;
using FontConverter.SharedLibrary.Models;
using Microsoft.AspNetCore.Components;
using Radzen;
using SkiaSharp;
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

    private float _GammaValue = 1.0f;

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
            RenderGlyph();
        }
    }

    private void OnDitherChanged(bool newValue)
    {
        if (MainViewModel.LastSelectedGlyph != null && MainViewModel.LastSelectedGlyph.Adjusments.Dither != newValue)
        {
            MainViewModel.LastSelectedGlyph.Adjusments.Dither = newValue;
            RenderGlyph();
        }
    }

    private void OnStyleChanged(object? newValue)
    {
        if (newValue != null && MainViewModel.LastSelectedGlyph != null && newValue is GLYPH_STYLE && MainViewModel.LastSelectedGlyph.Adjusments.Style != (GLYPH_STYLE)newValue)
        {
            MainViewModel.LastSelectedGlyph.Adjusments.Style = (GLYPH_STYLE)newValue;
            RenderGlyph();
        }
    }

    private void OnStrokeWidthChanged(int? newValue)
    {
        if (newValue != null && MainViewModel.LastSelectedGlyph != null && MainViewModel.LastSelectedGlyph.Adjusments.StrokeWidth != newValue)
        {
            MainViewModel.LastSelectedGlyph.Adjusments.StrokeWidth = (int)newValue;
            RenderGlyph();
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
            RenderGlyph();
        }
    }

    private void OnThresholdChanged(int? newValue)
    {
        if (newValue != null && MainViewModel.LastSelectedGlyph != null && MainViewModel.LastSelectedGlyph.Adjusments.Threshold != newValue)
        {
            MainViewModel.LastSelectedGlyph.Adjusments.Threshold = (int)newValue;
            RenderGlyph();
        }
    }

    private void RenderGlyph()
    {
        if (MainViewModel.LastSelectedGlyph == null || MainViewModel.OpenTypeFont == null || MainViewModel.OpenTypeFont.SKFont==null)
            return;
        using SKPaint paint = new()
        {
            IsAntialias = MainViewModel.LastSelectedGlyph.Adjusments.AntiAlias,
            IsDither = MainViewModel.LastSelectedGlyph.Adjusments.Dither,
            ColorFilter = MainViewModel.LastSelectedGlyph.Adjusments.ColorFilter ? SKColorFilter.CreateBlendMode(SKColors.Black, SKBlendMode.SrcIn) : null,
            Shader = MainViewModel.LastSelectedGlyph.Adjusments.Shader ? SKShader.CreateColor(SKColors.Black) : null,
            Style = (SKPaintStyle)MainViewModel.LastSelectedGlyph.Adjusments.Style,
            Color = SKColors.Black,
            MaskFilter = SKMaskFilter.CreateGamma(_GammaValue),
            StrokeWidth = MainViewModel.LastSelectedGlyph.Adjusments.StrokeWidth,
        };

        using SKFont svgFont = new SKFont(MainViewModel.OpenTypeFont.SKTypeface!, MainViewModel.LVGLFont.SVGTextSize);

        LVGLGlyphBitmapData renderData = RenderGlyphsToBitmapArrayHelper.RenderGlyphToBitmapArray(
            MainViewModel.OpenTypeFont.SKFont!, 
            paint,
            svgFont,
            (ushort)MainViewModel.LastSelectedGlyph.Index,
            MainViewModel.LVGLFont.FontSettings.FontSize,
            MainViewModel.LVGLFont.FontSettings.FontBitPerPixel,
            MainViewModel.LastSelectedGlyph.Adjusments.Threshold
            );
        var scale = MainViewModel.LVGLFont.FontSettings.FontSize / (double)MainViewModel.OpenTypeFont.HeadTable.UnitsPerEm;
        var glyphMetrics = MainViewModel.OpenTypeFont.HmtxTable.GlyphMetrics;
        MainViewModel.LastSelectedGlyph.Bitmap = renderData.Bitmap;
        MainViewModel.LastSelectedGlyph.Descriptor = new LVGLGlyphDescriptor
        {
            Width = renderData.Bounds.Width,
            Height = renderData.Bounds.Height,
            OffsetX = renderData.Bounds.Left,
            OffsetY = -renderData.Bounds.Bottom,
            AdvanceWidth = (int)Math.Ceiling(scale * glyphMetrics[MainViewModel.LastSelectedGlyph.Index].AdvanceWidth),
        };

        MainViewModel.OnGlyphPropertiesChanged?.Invoke(MainViewModel.LastSelectedGlyph.Index);
    }
}
