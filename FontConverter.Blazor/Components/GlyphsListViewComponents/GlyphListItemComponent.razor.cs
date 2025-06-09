using FontConverter.Blazor.Interfaces;
using FontConverter.Blazor.Models.GlyphsView;
using FontConverter.Blazor.Services;
using FontConverter.Blazor.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SkiaSharp;
using SkiaSharp.Views.Blazor;

namespace FontConverter.Blazor.Components.GlyphsListViewComponents;

public partial class GlyphListItemComponent : ComponentBase, IAsyncDisposable, IDisposable
{
    [Inject]
    public MainViewModel MainViewModel { get; set; } = default!;

    [Inject]
    public GlyphRenderQueueService GlyphRenderQueueService { get; set; } = default!;

    [Inject] 
    private IJSRuntime JSRuntime { get; set; } = default!;

    [Parameter]
    public EventCallback<(int GlyphID, bool IsSelected)> OnSelectionChanged { get; set; }

    [Parameter]
    public int GlyphId { get; set; } = -1;

    [Parameter]
    public int VisibilityTrackingID { get; set; }

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
    private string _HeaderTitle = string.Empty;
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

    private bool _IsRenderAllowed = false;
    private ElementReference _GlyphRef;
    private int _PrevGlyphId = -1;
    private int _PrevRowIndex = -1;
    private int _PrevColumnIndex = -1;
    private DotNetObjectReference<GlyphListItemComponent>? _DotNetRef;
    private int _PrevVisibilityTrackingID = -1;


    protected override void OnInitialized()
    {
        base.OnInitialized();
        UpdateItemMetrics();
        _Dispose = false;
        _PrevGlyphId = GlyphId;
        _PrevVisibilityTrackingID = VisibilityTrackingID;
        _DotNetRef = DotNetObjectReference.Create(this);
        GlyphRenderQueueService.OnRenderAllowed += HandleRenderAllowed;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        UpdateItemMetrics();
        if (firstRender)
        {
            if (_DotNetRef is not null && GlyphId >= 0)
            {
                await JSRuntime.InvokeVoidAsync("startGlyphVisibilityTracking", _GlyphRef, _DotNetRef, VisibilityTrackingID);
            }
        }
        
        _SKCanvasView?.Invalidate();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (GlyphId < 0)
            return;

        if (_PrevVisibilityTrackingID != VisibilityTrackingID)
        {
            UpdateItemMetrics();

            _IsRenderAllowed = false;

            //GlyphRenderQueueService.UnregisterGlyph(_PrevVisibilityTrackingID);
            //GlyphRenderQueueService.UnregisterGlyph(VisibilityTrackingID);

            //await JSRuntime.InvokeVoidAsync("stopGlyphVisibilityTracking", _PrevVisibilityTrackingID);
            //await JSRuntime.InvokeVoidAsync("stopGlyphVisibilityTracking", VisibilityTrackingID);

            if (_DotNetRef is not null)
                await JSRuntime.InvokeVoidAsync("startGlyphVisibilityTracking", _GlyphRef, _DotNetRef, VisibilityTrackingID);

            _PrevVisibilityTrackingID = VisibilityTrackingID;

            await InvokeAsync(StateHasChanged);
        }
        if (_PrevGlyphId != GlyphId)
        {
            UpdateItemMetrics();

            _PrevGlyphId = GlyphId;

            await InvokeAsync(StateHasChanged);
        }
        if (_IsRenderAllowed)
        {
            UpdateItemMetrics();
            _SKCanvasView?.Invalidate();
            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task ToggleSelection()
    {
        IsSelected = !IsSelected;
        await OnSelectionChanged.InvokeAsync((GlyphId, IsSelected));
    }

    private void UpdateItemMetrics()
    {
        var props = MainViewModel.GlyphViewItemPropertiesViewModel;
        _Padding = props.ItemPadding;
        _Zoom = props.Zoom;

        _ItemWidth = MainViewModel.GlyphItemWidth;
        _ItemHeight = MainViewModel.GlyphItemHeight;

        _CanvasWidth = _ItemWidth;
        _CanvasHeight = _ItemHeight - _HeaderHeight;

        _XAxis = (float)_CanvasHeight - (float)(props.BaseLine * _Zoom) - (float)(_Padding / 2.0f);
        if (props.XMin >= 0)
        {
            _YAxis = (float)(_Padding / 2.0f);
        }
        else
        {
            _YAxis = (float)(-props.XMin * _Zoom) + (float)(_Padding / 2.0f);
        }

        _BitPerPixel = (int)MainViewModel.FontSettingsViewModel.FontBitPerPixel;

        if (MainViewModel.GlyphsList.TryGetValue(GlyphId, out var glyphItem))
        {
            _GlyphPixels = glyphItem.Bitmap;
            _BitMapWidth = glyphItem.Descriptor.Width;
            _BitMapHeight = glyphItem.Descriptor.Height;
            _AdvanceWidth = glyphItem.Descriptor.AdvanceWidth * _Zoom;
            _BitmapXOffset = _YAxis + (float)(glyphItem.Descriptor.OffsetX * _Zoom);
            _BitmapYOffset = _XAxis - (float)((_BitMapHeight + glyphItem.Descriptor.OffsetY) * _Zoom);
            _HeaderTitle = glyphItem.Name;
        }
        else
        {
            _GlyphPixels = [];
            _BitMapWidth = 0;
            _BitMapHeight = 0;
            _AdvanceWidth = 0;
            _BitmapXOffset = 0;
            _BitmapYOffset = 0;
            _HeaderTitle = string.Empty;
        }
    }

    private void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        if (!_IsRenderAllowed)
            return;

        if (_Dispose) return;

        if (_SKCanvasView == null || e.Surface == null || e.Surface.Canvas == null)
            return;

        _SKSurface = e.Surface;
        _SKCanvas = e.Surface.Canvas;
        var info = e.Info;
        if (info.Width == 0 || info.Height == 0)
            return;

        
        
        PaintCanvas();

    }

    private void PaintCanvas()
    {

        if (_SKCanvasView is null) return;
        if (_SKSurface is null) return;
        if (_SKCanvas is null) return;

        UpdateItemMetrics();
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

                var left = (int)(offset.X + x * zoom);
                var top = (int)(offset.Y + y * zoom);
                var right = (int)(offset.X + (x + 1) * zoom);
                var bottom = (int)(offset.Y + (y + 1) * zoom);

                var rect = new SKRect(left, top, right, bottom);
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
        _Dispose = true;
        _IsRenderAllowed = false;
        GlyphRenderQueueService.UnregisterGlyph(VisibilityTrackingID);
        _ = JSRuntime.InvokeVoidAsync("stopGlyphVisibilityTracking", VisibilityTrackingID);
        _DotNetRef?.Dispose();
        _DotNetRef = null;
        GlyphRenderQueueService.OnRenderAllowed -= HandleRenderAllowed;
        _SKCanvasView = null;
    }

    public ValueTask DisposeAsync()
    {
        Dispose();
        return ValueTask.CompletedTask;
    }

    private void HandleRenderAllowed(int trackingID)
    {
        if (_Dispose || VisibilityTrackingID != trackingID)
            return;
        _IsRenderAllowed = true;
        InvokeAsync(StateHasChanged);
    }


    [JSInvokable]
    public async Task OnVisible(int trackingID)
    {
        if (_Dispose || VisibilityTrackingID != trackingID)
            return;
        GlyphRenderQueueService.EnqueueVisibleGlyph(trackingID);
        
    }

    [JSInvokable]
    public async Task OnInvisible(int trackingID)
    {
        if (_Dispose || VisibilityTrackingID != trackingID)
            return;
        GlyphRenderQueueService.UnregisterGlyph(trackingID);
    }
}
