using Microsoft.AspNetCore.Components;
using SkiaSharp.Views.Blazor;
using SkiaSharp;
using FontConverter.Blazor.ViewModels;
using FontConverter.Blazor.Interfaces;
using FontConverter.Blazor.Components.LeftSidebarComponents;
using System.Drawing;
using System.Data;

namespace FontConverter.Blazor.Components.GlyphsListViewComponents;

public partial class GlyphViewItemComponent : ComponentBase, IAsyncDisposable, IDisposable
{
    [Inject]
    public MainViewModel MainViewModel { get; set; } = default!;

    [Parameter]
    public EventCallback<(int GhlyphID, bool IsSelected)> OnSelectionChanged { get; set; }

    [Parameter]
    public EventCallback<int> OnGlyphRendered { get; set; }

    [Parameter]
    public int GlyphId { get; set; } = -1;

    private SKCanvasView? _SKCanvasView;
    private SKSurface? _SKSurface;
    private SKCanvas? _SKCanvas;

    private bool IsSelected { get; set; } = false;

    private int _Padding = 0;

    private int _ItemWidth;
    private int _ItemHeight;
    private int _Zoom;
    private int _HeaderHeight = 20;
    private float _CanvasWidth;
    private float _CanvasHeight;
    private byte[] _GlyphPixels = [];
    private int _BitMapWidth;
    private int _BitMapHeight;
    private float _XAxis;
    private float _YAxis;
    private int _AdvanceWidth;
    private int _BitPerPixel;
    private float _BitmapXOffset;
    private float _BitmapYOffset;

    private bool _Dispose = false;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        InvokeAsync(MeasureOverride);
        _Dispose = false;
    }

    protected override async Task OnParametersSetAsync()
    {
        await InvokeAsync(() => _SKCanvasView?.Invalidate());
    }

    protected async override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            await InvokeAsync(MeasureOverride);
    }

    private async Task ToggleSelection()
    {
        IsSelected = !IsSelected;
        await OnSelectionChanged.InvokeAsync((GlyphId, IsSelected));
    }

    private void MeasureOverride()
    {
        _Padding = MainViewModel.GlyphViewItemPropertiesViewModel.ItemPadding;
        _Zoom = MainViewModel.GlyphViewItemPropertiesViewModel.Zoom;

        _ItemWidth = MainViewModel.GlyphItemWidth;
        _ItemHeight = MainViewModel.GlyphItemHeight;

        _CanvasWidth = _ItemWidth;
        _CanvasHeight = _ItemHeight - _HeaderHeight;
        if (MainViewModel.LVGLFont.Glyphs.ContainsKey(GlyphId))
        {
            _GlyphPixels = MainViewModel.LVGLFont.Glyphs[GlyphId].Bitmap;
            _BitMapWidth = MainViewModel.LVGLFont.Glyphs[GlyphId].Descriptor.Width * _Zoom;
            _BitMapHeight = MainViewModel.LVGLFont.Glyphs[GlyphId].Descriptor.Height * _Zoom;

            _XAxis = (float)_CanvasHeight - (float)(MainViewModel.GlyphViewItemPropertiesViewModel.BaseLine * _Zoom) - (float)(_Padding / 2.0f);
            if (MainViewModel.GlyphViewItemPropertiesViewModel.XMin >= 0)
            {
                _YAxis = (float)(MainViewModel.GlyphViewItemPropertiesViewModel.ItemPadding / 2.0f);
            }
            else
            {
                _YAxis = (float)(-MainViewModel.GlyphViewItemPropertiesViewModel.XMin * _Zoom) + (float)(_Padding / 2.0f);
            }
            _AdvanceWidth = MainViewModel.LVGLFont.Glyphs[GlyphId].Descriptor.AdvanceWidth * _Zoom;
            _BitPerPixel = (int)MainViewModel.FontSettingsViewModel.FontBitPerPixel;

            _BitmapXOffset = _YAxis + (float)(MainViewModel.LVGLFont.Glyphs[GlyphId].Descriptor.OffsetX * _Zoom);
            _BitmapYOffset = _XAxis - (float)(_BitMapHeight + (MainViewModel.LVGLFont.Glyphs[GlyphId].Descriptor.OffsetY * _Zoom));
        }
    }

    private void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        _SKSurface = e.Surface;
        _SKCanvas = e.Surface.Canvas;
        var info = e.Info;
        if (info.Width == 0 || info.Height == 0)
            return;
        InvokeAsync(PaintCanvas);
    }

    private void PaintCanvas()
    {

        if (_SKCanvasView is null) return;
        if (_SKSurface is null) return;
        if (_SKCanvas is null) return;

        MeasureOverride();
        _SKCanvas.Clear(SKColors.White);

        using var mainRectPaint = new SKPaint
        {
            Color = new SKColor(0xF6, 0xEA, 0xCB, 0x66),
            Style = SKPaintStyle.Fill
        };
        _SKCanvas.DrawRect(0, 0, _CanvasWidth, _CanvasHeight, mainRectPaint);

        using var advanceWidthPaint = new SKPaint
        {
            Color = new SKColor(0xFF, 0xE3, 0xE3, 0x88),
            Style = SKPaintStyle.Fill
        };
        _SKCanvas.DrawRect(_YAxis, 0, _AdvanceWidth, _CanvasHeight, advanceWidthPaint);

        using var axisPaint = new SKPaint
        {
            Color = new SKColor(0xCD, 0x5C, 0x5C, (int)(255 * 0.25f)),
            StrokeWidth = 1,
            Style = SKPaintStyle.Stroke,
            IsAntialias = false,
        };
        _SKCanvas.DrawLine(0, _XAxis, _CanvasWidth, _XAxis, axisPaint); // X Axis
        _SKCanvas.DrawLine(_YAxis, 0, _YAxis, _CanvasHeight, axisPaint); // Y Axis

        RenderGlyphToCanvas(_SKCanvas, _GlyphPixels, _BitMapWidth, _BitMapHeight, _BitPerPixel, _Zoom, new SKPoint(_BitmapXOffset, _BitmapYOffset));
    }

    private void RenderGlyphToCanvas(SKCanvas canvas, byte[] bitmap, int width, int height, int bpp, int zoom, SKPoint offset)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                byte value = GetPixel(bitmap, width, bpp, x, y);
                if (value == 0)
                    continue;

                byte alpha = (byte)(value * 255 / ((1 << bpp) - 1));
                var paint = new SKPaint
                {
                    Color = new SKColor(0, 0, 0, alpha),
                    IsAntialias = false,
                    Style = SKPaintStyle.Fill
                };

                var rect = new SKRect(
                    offset.X + x * zoom,
                    offset.Y + y * zoom,
                    offset.X + (x + 1) * zoom,
                    offset.Y + (y + 1) * zoom);
                canvas.DrawRect(rect, paint);
            }
        }
    }

    private byte GetPixel(byte[] data, int width, int bpp, int x, int y)
    {
        int index = y * width + x;
        int pixelsPerByte = 8 / bpp;
        int byteIndex = index / pixelsPerByte;
        if (byteIndex >= data.Length)
            return 0;

        byte b = data[byteIndex];
        int bitOffset = bpp * (pixelsPerByte - 1 - (index % pixelsPerByte));
        return (byte)((b >> bitOffset) & ((1 << bpp) - 1));
    }

    public void Dispose()
    {
        if (_Dispose)
            return;
        _SKCanvas?.Dispose();
        _SKSurface?.Dispose();
        //_SKCanvasView?.Dispose();
        _SKCanvas = null;
        _SKSurface = null;
        _SKCanvasView = null;
        _Dispose = true;
    }

    public ValueTask DisposeAsync()
    {
        if (!_Dispose)
        {
            Dispose();
        }
        return ValueTask.CompletedTask;
    }
}
