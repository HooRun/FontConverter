using LVGLFontConverter.Library.Models;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Composition;
using Microsoft.Graphics.DirectX;
using Microsoft.UI;
using Microsoft.UI.Composition;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;

namespace LVGLFontConverter.Library;

public class GlyphBitmapControl : Panel
{
    private ContainerVisual _baseContainer;
    private ContainerVisual _headerContainer;
    private ContainerVisual _contentContainer;
    private ContainerVisual _adwContainer;
    private ContainerVisual _glyphContainer;
    private SpriteVisual _overlay;
    //private CompositionGraphicsDevice _graphicsDevice;
    private Compositor _compositor;
    private Canvas _inputOverlay;

    private InputNonClientPointerSource _pointerSource;
    private Point _lastPosition;
    private bool _isMouseDown;
    private bool _isInBounds;

    private LVGLFontGlyph _glyph;
    private byte[] _bitmapData = [];
    private int _bitmapWidth = 0;
    private int _bitmapHeight = 0;
    private int _bitsPerPixel = 0;
    private int _baseline  = 0;
    private int _lineHeight = 0;
    private int _maxCharWidth = 0;
    private int _headerWidth = 0;
    private int _headerHeight = 0;
    private int _contentWidth = 0;
    private int _content_Heigt = 0;
    private float _headerFontSize = 12;
    private int _ratio = 1;

    private bool isSelected = false;

    public int Index { get; set; }

    public GlyphBitmapControl()
    {

        this.Background = new SolidColorBrush(Colors.Transparent);
        


        MinWidth = 40;
        MinHeight = 40;
        Loaded += OnLoaded;

        IsTapEnabled = true;

        PointerEntered -= GlyphBitmapControl_PointerEntered;
        PointerEntered += GlyphBitmapControl_PointerEntered;

        PointerExited += GlyphBitmapControl_PointerExited;
        Tapped += GlyphBitmapControl_Tapped;
    }

    private void GlyphBitmapControl_Tapped(object sender, TappedRoutedEventArgs e)
    {
        if (isSelected)
        {
            _overlay.Brush = _compositor.CreateColorBrush(Colors.Transparent);
            isSelected = false;
        }
        else
        {
            _overlay.Brush = _compositor.CreateColorBrush(Color.FromArgb(80, 0, 0, 30));
            isSelected = true;
        }
    }

    private void GlyphBitmapControl_PointerExited(object sender, PointerRoutedEventArgs e)
    {
        _overlay.Brush = _compositor.CreateColorBrush(Colors.Transparent);
    }

    private void GlyphBitmapControl_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        _overlay.Brush = _compositor.CreateColorBrush(Color.FromArgb(30,0,0,30));
    }

    public GlyphBitmapControl(LVGLFontGlyph glyph) : this()
    {
        
        _glyph = glyph;
        _bitmapData = glyph.Bitmap;
        _bitmapWidth = glyph.BitmapWidth;
        _bitmapHeight = glyph.BitmapHeight;
        _bitsPerPixel = glyph.BitsPerPixel;
        _baseline = glyph.BaseLine;
        _lineHeight = glyph.LineHeight;
        _maxCharWidth = glyph.MaxCharWidth;
        Index = glyph.Index;
    }


    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        InitComposition();
    }

    private void InitComposition()
    {
        Visual visual = ElementCompositionPreview.GetElementVisual(this);
        _compositor = visual.Compositor;
        _baseContainer = _compositor.CreateContainerVisual();
        _headerContainer = _compositor.CreateContainerVisual();
        _contentContainer = _compositor.CreateContainerVisual();
        _adwContainer = _compositor.CreateContainerVisual();
        _glyphContainer = _compositor.CreateContainerVisual();        

        


        int widthPadding = (int)HeaderPadding.Left + (int)HeaderPadding.Right;
        int heightPadding = (int)HeaderPadding.Top + (int)HeaderPadding.Bottom;

        _headerWidth = _maxCharWidth + widthPadding;
        _headerHeight = (int)Math.Ceiling(MeasureText( "HEADER", _headerFontSize, _headerWidth).Height+heightPadding);
        _contentWidth = _headerWidth;
        _content_Heigt = Zoom * _lineHeight;

        if (_headerWidth < MinWidth)
        {
            _ratio = (int)Math.Ceiling(MinWidth / _headerWidth);
            _headerWidth = _headerWidth * _ratio;
            _content_Heigt = _content_Heigt * _ratio;
        }       

        Width = _headerWidth;
        Height = _content_Heigt + _headerHeight;









        Vector2 size = new((float)Width, (float)Height);
        float cornerRadius = 10f;
        float borderThickness = 0f;

        float innerX = borderThickness;
        float innerY = borderThickness;
        float innerWidth = Math.Max(0, size.X - 2 * borderThickness);
        float innerHeight = Math.Max(0, size.Y - 2 * borderThickness);

        float innerCornerRadius = Math.Max(0, cornerRadius - borderThickness / 2f);
        innerCornerRadius = Math.Min(innerCornerRadius, Math.Min(innerWidth, innerHeight) / 2f);

        // 1. رسم ماسک با Canvas
        var canvasDevice = CanvasDevice.GetSharedDevice();
        var graphicsDevice2 = CanvasComposition.CreateCompositionGraphicsDevice(_compositor, canvasDevice);
        var surface = graphicsDevice2.CreateDrawingSurface(
            new Size(size.X, size.Y),
            DirectXPixelFormat.B8G8R8A8UIntNormalized,
            DirectXAlphaMode.Premultiplied);

        var outer = CanvasGeometry.CreateRoundedRectangle(canvasDevice, 0, 0, size.X, size.Y, cornerRadius, cornerRadius);
        var inner = CanvasGeometry.CreateRoundedRectangle(canvasDevice,
            innerX, innerY,
            innerWidth, innerHeight,
            innerCornerRadius,
            innerCornerRadius);

        //var borderShape = outer.CombineWith(inner, Matrix3x2.Identity, CanvasGeometryCombine.Exclude);

        using (var ds = CanvasComposition.CreateDrawingSession(surface))
        {
            ds.Clear(Colors.Transparent);
            ds.FillGeometry(outer, Colors.White); // ناحیه‌ی ماسک‌شده
        }
        var maskBrush = _compositor.CreateSurfaceBrush(surface);

        //var borderSurface = graphicsDevice2.CreateDrawingSurface(
        //   new Size(size.X, size.Y),
        //   DirectXPixelFormat.B8G8R8A8UIntNormalized,
        //   DirectXAlphaMode.Premultiplied);

        //using (var ds = CanvasComposition.CreateDrawingSession(borderSurface))
        //{
        //    ds.Clear(Colors.Transparent);
        //    ds.FillGeometry(borderShape, Colors.Gray); 
        //}

        //var borderVisual = _compositor.CreateSpriteVisual();
        //borderVisual.Size = size;
        //borderVisual.Brush = _compositor.CreateSurfaceBrush(borderSurface);
        //_baseContainer.Children.InsertAtTop(borderVisual);

        var contentContainer = _compositor.CreateContainerVisual();
        contentContainer.Offset = Vector3.Zero;
        // محتوای داخلی به این اضافه می‌شن
        var maskedContentBrush = _compositor.CreateMaskBrush();
        maskedContentBrush.Mask = maskBrush;

        // محتوای داخل قاب رو به شکل یک لایه نمایش می‌دیم
        var contentVisual = _compositor.CreateSpriteVisual();
        contentVisual.Brush = maskedContentBrush;
        contentVisual.Size = size;
        contentVisual.Offset = Vector3.Zero;
        // اضافه کردن لایه‌ها به base
        _baseContainer.Children.InsertAtTop(contentVisual);

        var visualSurface = _compositor.CreateVisualSurface();
        visualSurface.SourceVisual = contentContainer;
        visualSurface.SourceSize = size;
        visualSurface.SourceOffset = Vector2.Zero;//new Vector2(borderThickness, borderThickness);

        var sourceBrush = _compositor.CreateSurfaceBrush(visualSurface);
        maskedContentBrush.Source = sourceBrush;

        var innerContent = _compositor.CreateContainerVisual();
        innerContent.Offset = new Vector3(borderThickness, borderThickness, 0);
        innerContent.Size = new Vector2(innerWidth, innerHeight);
        

        contentContainer.Children.InsertAtTop(innerContent);




        SpriteVisual bodyRect = _compositor.CreateSpriteVisual();
        bodyRect.Brush = _compositor.CreateColorBrush(Colors.WhiteSmoke);
        bodyRect.Size = new Vector2((float)Width, (float)Height);
        bodyRect.Offset = new Vector3(0, 0, 0);
        innerContent.Children.InsertAtTop(bodyRect);
        

        SpriteVisual headerRect = _compositor.CreateSpriteVisual();
        headerRect.Brush = _compositor.CreateColorBrush(Colors.LightSteelBlue);
        headerRect.Size = new Vector2(_headerWidth, _headerHeight);
        headerRect.Offset = new Vector3(0, 0, 0);
        _headerContainer.Children.InsertAtTop(headerRect);

        var device = CanvasDevice.GetSharedDevice();
        var graphicsDevice = CanvasComposition.CreateCompositionGraphicsDevice(_compositor, device);
        var truncatedText = TruncateText(_glyph.Name ?? "Glyph", _headerFontSize, _headerWidth-widthPadding);
        var textSize = MeasureText(truncatedText, _headerFontSize, _headerWidth - widthPadding);

        var drawingSurface = graphicsDevice.CreateDrawingSurface(
            textSize,
            DirectXPixelFormat.B8G8R8A8UIntNormalized,
            DirectXAlphaMode.Premultiplied);

        using (var session = CanvasComposition.CreateDrawingSession(drawingSurface))
        {
            session.Clear(Colors.Transparent);
            session.TextAntialiasing = CanvasTextAntialiasing.Auto;
            session.DrawText(
                truncatedText,
                new Vector2(0, 0),
                Colors.Black,
                new CanvasTextFormat()
                {
                    FontSize = _headerFontSize,
                    HorizontalAlignment = CanvasHorizontalAlignment.Left,
                });
        }

        SpriteVisual textSprite = _compositor.CreateSpriteVisual();

        int textLeft = (_headerWidth - (int)textSize.Width) / 2;
        int textTop = (int)Math.Floor(heightPadding / 2.0);
        textSprite.Size = new Vector2((float)textSize.Width, (float)textSize.Height);
        textSprite.Offset = new Vector3(textLeft, textTop, 0);
        CompositionSurfaceBrush surfaceBrush = _compositor.CreateSurfaceBrush(drawingSurface);
        textSprite.Brush = surfaceBrush;
        textSprite.IsPixelSnappingEnabled = true;
        _headerContainer.Children.InsertAtTop(textSprite);




        innerContent.Children.InsertAtTop(_headerContainer);
        innerContent.Children.InsertAtTop(_contentContainer);
        innerContent.Children.InsertAtTop(_adwContainer);
        innerContent.Children.InsertAtTop(_glyphContainer);





        _overlay = _compositor.CreateSpriteVisual();
        _overlay.Size = innerContent.Size;
        _overlay.Brush = _compositor.CreateColorBrush(Colors.Transparent); // ابتدا شفاف
        innerContent.Children.InsertAtTop(_overlay);


        ElementCompositionPreview.SetElementChildVisual(this, _baseContainer);

        

        

        
        this.SizeChanged += OnSizeChanged;
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (_baseContainer != null)
        {
            _baseContainer.Size = new Vector2((float)e.NewSize.Width, (float)e.NewSize.Height);
        }
    }

    private Size MeasureText(string text, float fontSize,float maxWidth)
    {
        var device = CanvasDevice.GetSharedDevice();
        var textFormat = new CanvasTextFormat()
        {
            FontSize = fontSize,
            HorizontalAlignment = CanvasHorizontalAlignment.Left,
            VerticalAlignment = CanvasVerticalAlignment.Center,
            WordWrapping = CanvasWordWrapping.NoWrap
        };

        using var textLayout = new CanvasTextLayout(
            device,
            text ?? string.Empty,
            textFormat,
            maxWidth,    // reasonable max width
            256      // reasonable max height
        );

        return new Size(
            (float)Math.Ceiling(textLayout.LayoutBounds.Width),
            (float)Math.Ceiling(textLayout.LayoutBounds.Height)
        );
    }

    private string TruncateText(string text, float fontSize, float maxWidth)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        var ellipsis = "...";
        var ellipsisSize = MeasureText(ellipsis, fontSize, maxWidth);

        if (MeasureText(text, fontSize, maxWidth).Width <= maxWidth)
            return text;

        for (int i = text.Length - 1; i > 0; i--)
        {
            var truncatedText = text.Substring(0, i) + ellipsis;
            if (MeasureText(truncatedText, fontSize, maxWidth).Width <= maxWidth)
                return truncatedText;
        }

        return ellipsis; 
    }



    public Thickness HeaderPadding
    {
        get { return (Thickness)GetValue(HeaderPaddingProperty); }
        set { SetValue(HeaderPaddingProperty, value); }
    }
    public static readonly DependencyProperty HeaderPaddingProperty =
        DependencyProperty.Register("GlyphHeaderPadding", typeof(Thickness), typeof(GlyphBitmapControl), new PropertyMetadata(new Thickness(2)));



    public int Zoom
    {
        get { return (int)GetValue(ZoomProperty); }
        set { SetValue(ZoomProperty, value); }
    }
    public static readonly DependencyProperty ZoomProperty =
        DependencyProperty.Register("GlyphZoom", typeof(int), typeof(GlyphBitmapControl), new PropertyMetadata(1));



    //public void RenderBitmap()
    //{
    //    Background = new SolidColorBrush(Colors.WhiteSmoke);
    //    Width = _Glyph.MaxCharWidth * PixelSize;
    //    Height = _Glyph.LineHeight * PixelSize;
    //    Children.Clear();
    //    TextBlock header = new();
    //    header.Text = _Glyph.Name;
    //    SetLeft(header, 0);
    //    SetTop(header, 0);
    //    //Children.Add(header);
    //    for (int j = 0; j < _Glyph.LineHeight; j++)
    //    {
    //        int adw = _Glyph.AdvanceWidth;
    //        if (adw < 0)
    //        {
    //            adw = 0;
    //        }
    //        for (int k = 0; k < adw; k++)
    //        {
    //            var rect = new Rectangle()
    //            {
    //                Width = PixelSize,
    //                Height = PixelSize,
    //                Fill = new SolidColorBrush(Colors.Gray)
    //            };
    //            SetLeft(rect, (k - _Glyph.YAxisPosition) * PixelSize);
    //            SetTop(rect, j * PixelSize);
    //            Children.Add(rect);
    //        }
    //    }

    //    // محور Y (X = 0)
    //    var lineY = new Rectangle
    //    {
    //        Width = 1,
    //        Height = Height,
    //        Fill = new SolidColorBrush(Colors.Red)
    //    };
    //    SetLeft(lineY, -_Glyph.YAxisPosition * PixelSize);
    //    SetTop(lineY, 0);
    //    Children.Add(lineY);

    //    // خط baseline
    //    var baselineY = Baseline * PixelSize;
    //    var lineBaseline = new Rectangle();
    //    lineBaseline.Width = Width;
    //    lineBaseline.Height = 1;
    //    lineBaseline.Fill = new SolidColorBrush(Colors.Red);

    //    SetLeft(lineBaseline, 0);
    //    SetTop(lineBaseline, Height - baselineY);
    //    Children.Add(lineBaseline);

    //    for (int y = 0; y < BitmapHeight; y++)
    //    {
    //        for (int x = 0; x < BitmapWidth; x++)
    //        {
    //            byte value = GetPixel(x, y);
    //            if (value > 0)
    //            {
    //                byte alpha = (byte)(255 * value / ((1 << BitsPerPixel) - 1));
    //                var rect = new Rectangle
    //                {
    //                    Width = PixelSize,
    //                    Height = PixelSize,
    //                    Fill = new SolidColorBrush(Color.FromArgb(alpha, 0, 0, 0))
    //                };
    //                SetLeft(rect, (x + _Glyph.OffsetX - _Glyph.YAxisPosition) * PixelSize);
    //                int cy = (_Glyph.LineHeight - (BitmapHeight + _Glyph.OffsetY)) + y;
    //                SetTop(rect, cy * PixelSize);
    //                Children.Add(rect);
    //            }
    //        }
    //    }






    //}

    //private byte GetPixel(int x, int y)
    //{
    //    int index = y * BitmapWidth + x;
    //    int pixelsPerByte = 8 / BitsPerPixel;
    //    int byteIndex = index / pixelsPerByte;

    //    if (byteIndex >= BitmapData.Count)
    //        return 0;

    //    byte b = BitmapData[byteIndex];
    //    int bitOffset = BitsPerPixel * (pixelsPerByte - 1 - (index % pixelsPerByte));
    //    return (byte)((b >> bitOffset) & ((1 << BitsPerPixel) - 1));
    //}
}
