using FontConverter.Blazor.EventsArgs;
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

    [Parameter]
    public int GlyphId { get; set; } = -1;

    private GlyphViewItemPropertiesViewModel _Dimentions => MainViewModel.GlyphViewItemPropertiesViewModel;

    private SKCanvasView? _SKCanvasView;

    private bool _IsSelected { get; set; } = false;
    private bool _IsHovered { get; set; } = false;
    private bool _LastSelected { get; set; } = false;

    private string _HeaderTitle = string.Empty;
    private byte[] _GlyphPixels = [];
    private int _BitMapWidth;
    private int _BitMapHeight;
    private int _AdvanceWidth;
    private int _BitPerPixel;
    private float _BitmapXOffset;
    private float _BitmapYOffset;

    private bool _IsDisposed = false;
    private ElementReference _GlyphRef;
    private int _PrevGlyphId = -1;
    private CancellationTokenSource? _ClickCts;


    protected override void OnInitialized()
    {
        base.OnInitialized();
        UpdateItemMetrics();
        _IsDisposed = false;
        _PrevGlyphId = GlyphId;
        if (MainViewModel.GlyphsList.TryGetValue(GlyphId, out var glyphItem))
        {
            _IsSelected = glyphItem.IsSelected;
            _IsHovered = glyphItem.IsHovered;
            _LastSelected = glyphItem.LastSelected;
        }
        MainViewModel.OnGlyphSelectionChanged += UpdateSelectionStatus;
        MainViewModel.OnGlyphZoomChanged += GlyphZoomChanged;
        MainViewModel.OnLastSelectedGlyphChanged += LastSelectedGlyphChanged;
        MainViewModel.OnGlyphPropertiesChanged += PropertiesChanged;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        UpdateItemMetrics();
        if (firstRender)
        {
            if (MainViewModel.GlyphsList.TryGetValue(GlyphId, out var glyphItem))
            {
                _IsSelected = glyphItem.IsSelected;
                _IsHovered = glyphItem.IsHovered;
                _LastSelected = glyphItem.LastSelected;
            }
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        bool glyphIdChanged = _PrevGlyphId != GlyphId;
        if (glyphIdChanged)
        {
            _PrevGlyphId = GlyphId;
            if (MainViewModel.GlyphsList.TryGetValue(GlyphId, out var glyphItem))
            {
                _IsSelected = glyphItem.IsSelected;
                _IsHovered = glyphItem.IsHovered;
                _LastSelected = glyphItem.LastSelected;
            }
        }

        if (glyphIdChanged)
        {
            UpdateItemMetrics();
            _SKCanvasView?.Invalidate();
            await InvokeAsync(StateHasChanged);
        }
    }
    
    private async Task OnClick()
    {
        _ClickCts?.Cancel();
        _ClickCts = new CancellationTokenSource();
        try
        {
            await Task.Delay(500, _ClickCts.Token);
            _LastSelected = !_LastSelected;
            if (MainViewModel.GlyphsList.TryGetValue(GlyphId, out var glyphItem))
            {
                glyphItem.LastSelected = _LastSelected;
            }
            MainViewModel.LastSelectedGlyphChanged(GlyphId, _LastSelected);
            await InvokeAsync(StateHasChanged);
        }
        catch (TaskCanceledException)
        {

        }
    }

    private async Task ToggleSelection()
    {
        _ClickCts?.Cancel();
        _IsSelected = !_IsSelected;       
        if (MainViewModel.GlyphsList.TryGetValue(GlyphId, out var glyphItem))
        {
            glyphItem.IsSelected = _IsSelected;
        }
        MainViewModel.GlyphSelectionChanged(GlyphId, _IsSelected);
        await InvokeAsync(StateHasChanged);
    }

    private async Task OnMuseEnter()
    {
        if (MainViewModel.GlyphsList.TryGetValue(GlyphId, out var glyphItem))
        {
            _IsHovered = true;
            glyphItem.IsHovered = true;
            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task OnMouseLeave()
    {
        if (MainViewModel.GlyphsList.TryGetValue(GlyphId, out var glyphItem))
        {
            _IsHovered = false;
            glyphItem.IsHovered = false;
            await InvokeAsync(StateHasChanged);
        }
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
            _IsSelected = glyphItem.IsSelected;
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
        if (_IsDisposed) return;

        if (_SKCanvasView == null || e.Surface == null || e.Surface.Canvas == null)
            return;

        var info = e.Info;
        if (info.Width == 0 || info.Height == 0)
            return;

        InvokeAsync(() => PaintCanvas(e.Surface.Canvas));
    }

    private void PaintCanvas(SKCanvas canvas)
    {
        UpdateItemMetrics();

        canvas.Clear(SKColors.White);

        using var mainRectPaint = new SKPaint
        {
            Color = new SKColor(0xF6, 0xEA, 0xCB, 0x66),
            Style = SKPaintStyle.Fill
        };
        canvas.DrawRect(0, 0, _Dimentions.CanvasWidth, _Dimentions.CanvasHeight, mainRectPaint);

        using var advanceWidthPaint = new SKPaint
        {
            Color = new SKColor(0xFF, 0xE3, 0xE3, 0x88),
            Style = SKPaintStyle.Fill
        };
        canvas.DrawRect(_Dimentions.YAxis, 0, _AdvanceWidth, _Dimentions.CanvasHeight, advanceWidthPaint);

        using var axisPaint = new SKPaint
        {
            Color = new SKColor(0xCD, 0x5C, 0x5C, (int)(255 * 0.25f)),
            StrokeWidth = 1,
            Style = SKPaintStyle.Stroke,
            IsAntialias = false
        };
        canvas.DrawLine(0, _Dimentions.XAxis, _Dimentions.CanvasWidth, _Dimentions.XAxis, axisPaint);
        canvas.DrawLine(_Dimentions.YAxis, 0, _Dimentions.YAxis, _Dimentions.CanvasHeight, axisPaint);

        using var glyphBoxPaint = new SKPaint
        {
            Color = new SKColor(0xB2, 0xA5, 0xFF, 0xF0),
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1,
            IsAntialias = false,
            PathEffect = SKPathEffect.CreateDash([2, 2], 0)
        };
        canvas.DrawRect(
            _BitmapXOffset,
            _BitmapYOffset,
            _BitMapWidth * _Dimentions.Zoom,
            _BitMapHeight * _Dimentions.Zoom,
            glyphBoxPaint);

        DrawGlyphBitmap(canvas, _GlyphPixels, _BitMapWidth, _BitMapHeight, _BitPerPixel, _Dimentions.Zoom, new SKPoint(_BitmapXOffset, _BitmapYOffset));
    }


    private void DrawGlyphBitmap(SKCanvas canvas, byte[] bitmap, int width, int height, int bpp, int zoom, SKPoint offset)
    {
        int stride = (width * bpp + 7) / 8;
        var pixelPaint = new SKPaint
        {
            IsAntialias = false,
            Style = SKPaintStyle.Fill
        };

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                byte value = GetPixel(bitmap, stride, bpp, x, y);
                if (value == 0) continue;

                byte alpha = (byte)(value * 255 / ((1 << bpp) - 1));
                pixelPaint.Color = new SKColor(0, 0, 0, alpha);

                float left = offset.X + x * zoom;
                float top = offset.Y + y * zoom;
                float size = zoom;

                canvas.DrawRect(left, top, size, size, pixelPaint);
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

        MainViewModel.OnGlyphSelectionChanged -= UpdateSelectionStatus;
        MainViewModel.OnGlyphZoomChanged -= GlyphZoomChanged;
        MainViewModel.OnLastSelectedGlyphChanged -= LastSelectedGlyphChanged;
        MainViewModel.OnGlyphPropertiesChanged -= PropertiesChanged;

        _SKCanvasView = null;
    }

    public ValueTask DisposeAsync()
    {
        Dispose();
        return ValueTask.CompletedTask;
    }

    private void UpdateSelectionStatus(GlyphSelectionChangedEventArgs selectionArgs)
    {
        if (GlyphId == selectionArgs.GlyphID && _IsSelected != selectionArgs.Selected)
        {
            _IsSelected = selectionArgs.Selected;
            StateHasChanged();
        }
    }

    private void GlyphZoomChanged()
    {
        UpdateItemMetrics();
        _SKCanvasView?.Invalidate();
        StateHasChanged();
    }

    private void LastSelectedGlyphChanged(LastSelectedGlyphEventArgs selectionArgs)
    {
        if (selectionArgs.Glyph.Index == GlyphId)
        {
            _LastSelected = selectionArgs.Selected;
            StateHasChanged();
        }
    }

    public void PropertiesChanged(int glyphID)
    {
        if (GlyphId == glyphID)
        {
            UpdateItemMetrics();
            _SKCanvasView?.Invalidate();
            StateHasChanged();
        }
    }
}
