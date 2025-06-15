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
    public int GlyphId { get; set; } = -1;

    [Parameter]
    public int VisibilityTrackingID { get; set; }

    private GlyphViewItemPropertiesViewModel _Dimentions => MainViewModel.GlyphViewItemPropertiesViewModel;

    private SKCanvasView? _SKCanvasView;

    private bool IsSelected { get; set; } = false;

    private string _HeaderTitle = string.Empty;
    private byte[] _GlyphPixels = [];
    private int _BitMapWidth;
    private int _BitMapHeight;
    private int _AdvanceWidth;
    private int _BitPerPixel;
    private float _BitmapXOffset;
    private float _BitmapYOffset;

    private bool _IsDisposed = false;
    private bool _IsRenderAllowed = false;
    private bool _IsRendered = false;
    private ElementReference _GlyphRef;
    private int _PrevGlyphId = -1;
    private DotNetObjectReference<GlyphListItemComponent>? _DotNetRef;
    private int _PrevVisibilityTrackingID = -1;


    protected override void OnInitialized()
    {
        base.OnInitialized();
        UpdateItemMetrics();
        _IsDisposed = false;
        _IsRenderAllowed = false;
        _PrevGlyphId = GlyphId;
        _PrevVisibilityTrackingID = VisibilityTrackingID;
        if (MainViewModel.GlyphsList.TryGetValue(GlyphId, out var glyphItem))
        {
            IsSelected = glyphItem.IsSelected;
        }
        GlyphRenderQueueService.OnRenderAllowed += HandleRenderAllowed;
        MainViewModel.OnSingleGlyphSelectionChanged += UpdateSelectionStatus;
        MainViewModel.OnGlyphZoomChanged += GlyphZoomChanged;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        UpdateItemMetrics();
        if (firstRender)
        {
            _IsRenderAllowed = false;
            if (MainViewModel.GlyphsList.TryGetValue(GlyphId, out var glyphItem))
            {
                IsSelected = glyphItem.IsSelected;
            }
            _DotNetRef ??= DotNetObjectReference.Create(this);
            await JSRuntime.InvokeVoidAsync("startGlyphVisibilityTracking", _GlyphRef, _DotNetRef, VisibilityTrackingID);
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        //if (GlyphId < 0)
        //{
        //    //_IsRenderAllowed = false;

        //    GlyphRenderQueueService.UnregisterGlyph(_PrevVisibilityTrackingID);
        //    await JSRuntime.InvokeVoidAsync("stopGlyphVisibilityTracking", _PrevVisibilityTrackingID);
            
        //    _PrevVisibilityTrackingID = VisibilityTrackingID;
        //    _PrevGlyphId = GlyphId;
        //    return;
        //}

        bool glyphIdChanged = _PrevGlyphId != GlyphId;
        bool trackingIdChanged = _PrevVisibilityTrackingID != VisibilityTrackingID;

        if (trackingIdChanged)
        {
            //_IsRenderAllowed = false;

            GlyphRenderQueueService.UnregisterGlyph(_PrevVisibilityTrackingID);
            await JSRuntime.InvokeVoidAsync("stopGlyphVisibilityTracking", _PrevVisibilityTrackingID);

            if (_DotNetRef is not null)
            {
                await JSRuntime.InvokeVoidAsync("startGlyphVisibilityTracking", _GlyphRef, _DotNetRef, VisibilityTrackingID);
            }

            _PrevVisibilityTrackingID = VisibilityTrackingID;
        }

        if (glyphIdChanged)
        {
            _PrevGlyphId = GlyphId;
            if (MainViewModel.GlyphsList.TryGetValue(GlyphId, out var glyphItem))
            {
                IsSelected = glyphItem.IsSelected;
            }
        }

        if (glyphIdChanged || trackingIdChanged || _IsRenderAllowed)
        {
            UpdateItemMetrics();
            _SKCanvasView?.Invalidate();
            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task ToggleSelection()
    {
        IsSelected = !IsSelected;
        if (MainViewModel.GlyphsList.TryGetValue(GlyphId, out var glyphItem))
        {
            glyphItem.IsSelected = IsSelected;
        }
        MainViewModel.GlyphSelectionChanged(GlyphId, IsSelected);
        await InvokeAsync(StateHasChanged);
    }

    private void UpdateItemMetrics()
    {
        _BitPerPixel = (int)MainViewModel.FontSettingsViewModel.FontBitPerPixel;

        if (MainViewModel.GlyphsList.TryGetValue(GlyphId, out var glyphItem))
        {
            _GlyphPixels = glyphItem.Bitmap;
            _BitMapWidth = glyphItem.Descriptor.Width;
            _BitMapHeight = glyphItem.Descriptor.Height;
            _AdvanceWidth = glyphItem.Descriptor.AdvanceWidth * _Dimentions.Zoom;
            _BitmapXOffset = _Dimentions.YAxis + (float)(glyphItem.Descriptor.OffsetX * _Dimentions.Zoom);
            _BitmapYOffset = _Dimentions.XAxis - (float)((_BitMapHeight + glyphItem.Descriptor.OffsetY) * _Dimentions.Zoom);
            _HeaderTitle = glyphItem.Name;
            IsSelected = glyphItem.IsSelected;
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

        if (_IsDisposed) return;

        if (_SKCanvasView == null || e.Surface == null || e.Surface.Canvas == null)
            return;

        var info = e.Info;
        if (info.Width == 0 || info.Height == 0)
            return;

        PaintCanvas(e.Surface.Canvas);
    }

    private void PaintCanvas(SKCanvas canvas)
    {
        UpdateItemMetrics();

        canvas.Clear(SKColors.White);

        using SKPaint mainRectPaint = new SKPaint
        {
            Color = new SKColor(0xF6, 0xEA, 0xCB, 0x66),
            Style = SKPaintStyle.Fill
        };
        canvas.DrawRect(0, 0, _Dimentions.CanvasWidth, _Dimentions.CanvasHeight, mainRectPaint);

        using SKPaint advanceWidthPaint = new SKPaint
        {
            Color = new SKColor(0xFF, 0xE3, 0xE3, 0x88),
            Style = SKPaintStyle.Fill
        };
        canvas.DrawRect(_Dimentions.YAxis, 0, _AdvanceWidth, _Dimentions.CanvasHeight, advanceWidthPaint);

        using SKPaint axisPaint = new SKPaint
        {
            Color = new SKColor(0xCD, 0x5C, 0x5C, (int)(255 * 0.25f)),
            StrokeWidth = 1,
            Style = SKPaintStyle.Stroke,
            IsAntialias = false,
        };
        canvas.DrawLine(0, _Dimentions.XAxis, _Dimentions.CanvasWidth, _Dimentions.XAxis, axisPaint); // X Axis
        canvas.DrawLine(_Dimentions.YAxis, 0, _Dimentions.YAxis, _Dimentions.CanvasHeight, axisPaint); // Y Axis

        DrawGlyphBitmapOnCanvas(canvas, _GlyphPixels, _BitMapWidth, _BitMapHeight, _BitPerPixel, _Dimentions.Zoom, new SKPoint(_BitmapXOffset, _BitmapYOffset));
    }

    private void DrawGlyphBitmapOnCanvas(SKCanvas canvas, byte[] bitmap, int width, int height, int bpp, int zoom, SKPoint offset)
    {
        int stride = (width * bpp + 7) / 8; 

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                byte value = GetPixel(bitmap, stride, bpp, x, y);
                if (value == 0) continue;

                byte alpha = (byte)(value * 255 / ((1 << bpp) - 1));

                using SKPaint pixelPaint = new SKPaint
                {
                    Color = new SKColor(0, 0, 0, alpha),
                    IsAntialias = false,
                    Style = SKPaintStyle.Fill
                };

                float left = offset.X + x * zoom;
                float top = offset.Y + y * zoom;
                float right = offset.X + (x + 1) * zoom;
                float bottom = offset.Y + (y + 1) * zoom;

                canvas.DrawRect(new SKRect(left, top, right, bottom), pixelPaint);
            }
        }
    }

    private byte GetPixel(byte[] data, int stride, int bpp, int x, int y)
    {
        int bitsPerRow = stride * 8;
        int bitIndex = y * bitsPerRow + x * bpp;
        int byteIndex = bitIndex / 8;
        int bitOffset = 8 - bpp - (bitIndex % 8); 

        if (byteIndex < 0 || byteIndex >= data.Length || bitOffset < 0)
            return 0;

        byte b = data[byteIndex];
        return (byte)((b >> bitOffset) & ((1 << bpp) - 1));
    }

    public void Dispose()
    {
        if (_IsDisposed)
            return;

        _IsDisposed = true;
        _IsRenderAllowed = false;

        MainViewModel.OnSingleGlyphSelectionChanged -= UpdateSelectionStatus;
        GlyphRenderQueueService.OnRenderAllowed -= HandleRenderAllowed;
        MainViewModel.OnGlyphZoomChanged -= GlyphZoomChanged;
        GlyphRenderQueueService.UnregisterGlyph(VisibilityTrackingID);

        _ = JSRuntime.InvokeVoidAsync("stopGlyphVisibilityTracking", VisibilityTrackingID);

        _DotNetRef?.Dispose();
        _DotNetRef = null;

        _SKCanvasView = null;
    }

    public ValueTask DisposeAsync()
    {
        Dispose();
        return ValueTask.CompletedTask;
    }

    private void HandleRenderAllowed(int trackingID)
    {
        if (_IsDisposed)
            return;

        if (trackingID == VisibilityTrackingID)
        {
            _IsRenderAllowed = true;

            _ = InvokeAsync(() =>
            {
                _SKCanvasView?.Invalidate();
                StateHasChanged();
            });
        }
    }

    [JSInvokable]
    public async Task OnVisible(int trackingID)
    {
        if (_IsDisposed || VisibilityTrackingID != trackingID)
            return;

        GlyphRenderQueueService.EnqueueVisibleGlyph(trackingID);
    }

    [JSInvokable]
    public async Task OnInvisible(int trackingID)
    {
        if (_IsDisposed || VisibilityTrackingID != trackingID)
            return;

        GlyphRenderQueueService.UnregisterGlyph(trackingID);
    }


    private void UpdateSelectionStatus((int GlyphID, bool Selected) selectionArgs)
    {
        if (GlyphId == selectionArgs.GlyphID && IsSelected != selectionArgs.Selected)
        {
            IsSelected = selectionArgs.Selected;
            StateHasChanged();
        }
    }

    private void GlyphZoomChanged()
    {
        UpdateItemMetrics();
        _SKCanvasView.Invalidate();
        StateHasChanged();
    }
}
