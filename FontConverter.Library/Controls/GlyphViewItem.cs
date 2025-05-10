using LVGLFontConverter.Library.Models;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Composition;
using Microsoft.Graphics.DirectX;
using Microsoft.UI;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LVGLFontConverter.Library;

public class GlyphViewItem : Panel, IDisposable
{
    public GlyphViewItem()
    {
        //this.DefaultStyleKey = typeof(GlyphViewItem);
        Background = new SolidColorBrush(Colors.Transparent);
        RegisterPropertyChangedCallback(BackgroundProperty, OnBackgroundChanged);
        IsTapEnabled = true;
        IsHitTestVisible = true;
        ManipulationMode = ManipulationModes.All;
        HorizontalAlignment = HorizontalAlignment.Left;
        VerticalAlignment = VerticalAlignment.Top;

        UnregisterEvents();
        RegisterEvents();

        CreateVisual();
    }

    public GlyphViewItem(LVGLFontGlyph glyph) : this()
    {
        Glyph = glyph;
        ToolTipText = Glyph.Name;
    }

    #region Internal Events
    protected override Size ArrangeOverride(Size finalSize)
    {
        ResizeVisual(finalSize);
        ArrangeVisual();
        RenderVisual();

        return base.ArrangeOverride(finalSize);
    }

    private void OnTapped(object sender, TappedRoutedEventArgs e)
    {
        IsSelected = !IsSelected;
        GlyphTapped?.Invoke(this, e);
    }

    private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
    {
        _overlayPointerEnter.Brush = _compositor.CreateColorBrush(HoverColor);
        GlyphPointerEntered?.Invoke(this, e);
    }

    private void OnPointerExited(object sender, PointerRoutedEventArgs e)
    {
        _overlayPointerEnter.Brush = _compositor.CreateColorBrush(Colors.Transparent);
        GlyphPointerExited?.Invoke(this, e);
    }

    private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
    {
        GlyphPointerPressed?.Invoke(this, e);
    }

    private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
    {
        GlyphPointerReleased?.Invoke(this, e);
    }

    private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
    {
        GlyphPointerMoved?.Invoke(this, e);
    }

    private void OnRightTapped(object sender, RightTappedRoutedEventArgs e)
    {
        GlyphRightTapped?.Invoke(this, e);
    }

    private void OnDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        GlyphDoubleTapped?.Invoke(this, e);
    }
    #endregion Internal Events

    #region Dependency Properties
    public LVGLFontGlyph Glyph
    {
        get { return (LVGLFontGlyph)GetValue(GlyphProperty); }
        set { SetValue(GlyphProperty, value); }
    }
    public static readonly DependencyProperty GlyphProperty =
        DependencyProperty.Register("Glyph", typeof(LVGLFontGlyph), typeof(GlyphViewItem), new PropertyMetadata(new LVGLFontGlyph(), OnGlyphChanged));
    private static void OnGlyphChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is GlyphViewItem glyphViewItem && glyphViewItem is not null)
        {
            glyphViewItem.Glyph = (LVGLFontGlyph)e.NewValue;
            string toolTip = $"Glyph Index: {glyphViewItem.Glyph.Index.ToString()}\n" +
                $"Glyph Name: {glyphViewItem.Glyph.Name}\n" +
                $"Glyph UniCode(s):\n{string.Join(Environment.NewLine, glyphViewItem.Glyph.Unicodes.Select(item => string.Format("  U+{0:X4}: {1}", item.CodePoint, item.Name)))}";
            glyphViewItem.ToolTipText = toolTip;
        }
    }

    public int Zoom
    {
        get { return (int)GetValue(ZoomProperty); }
        set
        {
            if (value < 1) value = 1;
            if (value > 100) value = 100;
            SetValue(ZoomProperty, value);
        }
    }
    public static readonly DependencyProperty ZoomProperty =
        DependencyProperty.Register("Zoom", typeof(int), typeof(GlyphViewItem), new PropertyMetadata(1));

    public float CornerRadius
    {
        get { return (float)GetValue(CornerRadiusProperty); }
        set { SetValue(CornerRadiusProperty, value); }
    }
    public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.Register("GlyphCornerRadius", typeof(float), typeof(GlyphViewItem), new PropertyMetadata(5.0f, OnCornerRadiusChanged));
    private static void OnCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is GlyphViewItem glyphViewItem && glyphViewItem is not null)
        {
            glyphViewItem.InvalidateArrange();
        }
    }

    public float BorderWidth
    {
        get { return (float)GetValue(BorderWidthProperty); }
        set { SetValue(BorderWidthProperty, value); }
    }
    public static readonly DependencyProperty BorderWidthProperty =
        DependencyProperty.Register("GlyphBorderWidth", typeof(float), typeof(GlyphViewItem), new PropertyMetadata(2.0f, OnBorderWidthChanged));
    private static void OnBorderWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is GlyphViewItem glyphViewItem && glyphViewItem is not null)
        {
            glyphViewItem.InvalidateArrange();
        }
    }

    public Color BorderColor
    {
        get { return (Color)GetValue(BorderColorProperty); }
        set { SetValue(BorderColorProperty, value); }
    }
    public static readonly DependencyProperty BorderColorProperty =
        DependencyProperty.Register("GlyphBorderColor", typeof(Color), typeof(GlyphViewItem), new PropertyMetadata(Colors.Transparent, OnBorderColorChanged));
    private static void OnBorderColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is GlyphViewItem glyphViewItem && glyphViewItem is not null)
        {
            if (glyphViewItem._isLoaded)
            {
                glyphViewItem.CreateBaseContinaerBorder();
            }
        }
    }

    public bool IsSelected
    {
        get { return (bool)GetValue(IsSelectedProperty); }
        set { SetValue(IsSelectedProperty, value); }
    }
    public static readonly DependencyProperty IsSelectedProperty =
        DependencyProperty.Register("IsSelected", typeof(bool), typeof(GlyphViewItem), new PropertyMetadata(false, OnIsSelectedChanged));
    private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is GlyphViewItem glyphViewItem && glyphViewItem is not null)
        {
            
                glyphViewItem.GlyphSelectionChanged?.Invoke(glyphViewItem, (bool)e.NewValue);
                if ((bool)e.NewValue)
                {
                    //glyphViewItem._overlayTapped.Brush = glyphViewItem._compositor.CreateColorBrush(glyphViewItem.GlyphSelectedColor);
                    glyphViewItem.BorderColor = Colors.CornflowerBlue;
                    glyphViewItem.CreateBaseContinaerBorder();
                }
                else
                {
                    glyphViewItem.BorderColor = Colors.Transparent;
                    glyphViewItem.CreateBaseContinaerBorder();
                    //glyphViewItem._overlayTapped.Brush = glyphViewItem._compositor.CreateColorBrush(Colors.Transparent);
                }

        }
    }

    public Color HoverColor
    {
        get { return (Color)GetValue(HoverColorProperty); }
        set { SetValue(HoverColorProperty, value); }
    }
    public static readonly DependencyProperty HoverColorProperty =
        DependencyProperty.Register("GlyphHoverColor", typeof(Color), typeof(GlyphViewItem), new PropertyMetadata(Color.FromArgb(15, 0, 0, 30)));

    public Color SelectedColor
    {
        get { return (Color)GetValue(SelectedColorProperty); }
        set { SetValue(SelectedColorProperty, value); }
    }
    public static readonly DependencyProperty SelectedColorProperty =
        DependencyProperty.Register("GlyphSelectedColor", typeof(Color), typeof(GlyphViewItem), new PropertyMetadata(Color.FromArgb(15, 0, 0, 30)));

    public float HeaderFontSize
    {
        get { return (float)GetValue(HeaderFontSizeProperty); }
        set { SetValue(HeaderFontSizeProperty, value); }
    }
    public static readonly DependencyProperty HeaderFontSizeProperty =
        DependencyProperty.Register("GlyphHeaderFontSize", typeof(float), typeof(GlyphViewItem), new PropertyMetadata(12.0f));

    public Color HeaderBackground
    {
        get { return (Color)GetValue(HeaderBackgroundProperty); }
        set { SetValue(HeaderBackgroundProperty, value); }
    }
    public static readonly DependencyProperty HeaderBackgroundProperty =
        DependencyProperty.Register("GlyphHeaderBackground", typeof(Color), typeof(GlyphViewItem), new PropertyMetadata(Colors.LightSteelBlue));

    public Color HeaderForeground
    {
        get { return (Color)GetValue(HeaderForegroundProperty); }
        set { SetValue(HeaderForegroundProperty, value); }
    }
    public static readonly DependencyProperty HeaderForegroundProperty =
        DependencyProperty.Register("GlyphHeaderForeground", typeof(Color), typeof(GlyphViewItem), new PropertyMetadata(Colors.Black));

    public Thickness HeaderPadding
    {
        get { return (Thickness)GetValue(HeaderPaddingProperty); }
        set { SetValue(HeaderPaddingProperty, value); }
    }
    public static readonly DependencyProperty HeaderPaddingProperty =
        DependencyProperty.Register("GlyphHeaderPadding", typeof(Thickness), typeof(GlyphViewItem), new PropertyMetadata(new Thickness(2)));

    public Thickness ContentPadding
    {
        get { return (Thickness)GetValue(ContentPaddingProperty); }
        set { SetValue(ContentPaddingProperty, value); }
    }
    public static readonly DependencyProperty ContentPaddingProperty =
        DependencyProperty.Register("GlyphContentPadding", typeof(Thickness), typeof(GlyphViewItem), new PropertyMetadata(new Thickness(5.0)));

    public Color Foreground
    {
        get { return (Color)GetValue(ForegroundProperty); }
        set { SetValue(ForegroundProperty, value); }
    }
    public static readonly DependencyProperty ForegroundProperty =
        DependencyProperty.Register("Foreground", typeof(Color), typeof(GlyphViewItem), new PropertyMetadata(Colors.Black));

    private void OnBackgroundChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (sender is GlyphViewItem glyphViewItem && glyphViewItem is not null)
        {
            if (glyphViewItem.Background is SolidColorBrush solidBrush)
            {
                if (solidBrush.Color != Colors.Transparent)
                {
                    Background = new SolidColorBrush(Colors.Transparent);
                    glyphViewItem._backgroundColor = solidBrush.Color;
                    if (glyphViewItem._isLoaded)
                    {
                        _backgroundVisual.Brush = _compositor.CreateColorBrush(glyphViewItem._backgroundColor);
                    }
                }
            }
            else
            {
                Background = new SolidColorBrush(Colors.Transparent);
                glyphViewItem._backgroundColor = Colors.Transparent;
                if (glyphViewItem._isLoaded)
                {
                    _backgroundVisual.Brush = _compositor.CreateColorBrush(glyphViewItem._backgroundColor);
                }
            }
        }

    }
    #endregion Dependency Properties

    #region Event Handlers
    public event TypedEventHandler<GlyphViewItem, TappedRoutedEventArgs> GlyphTapped;
    public event TypedEventHandler<GlyphViewItem, RightTappedRoutedEventArgs> GlyphRightTapped;
    public event TypedEventHandler<GlyphViewItem, DoubleTappedRoutedEventArgs> GlyphDoubleTapped;
    public event TypedEventHandler<GlyphViewItem, PointerRoutedEventArgs> GlyphPointerEntered;
    public event TypedEventHandler<GlyphViewItem, PointerRoutedEventArgs> GlyphPointerExited;
    public event TypedEventHandler<GlyphViewItem, PointerRoutedEventArgs> GlyphPointerPressed;
    public event TypedEventHandler<GlyphViewItem, PointerRoutedEventArgs> GlyphPointerReleased;
    public event TypedEventHandler<GlyphViewItem, PointerRoutedEventArgs> GlyphPointerMoved;
    public event TypedEventHandler<GlyphViewItem, bool> GlyphSelectionChanged;
    #endregion Event Handlers

    #region Private Variables
    private Visual _visualElement;
    private Compositor _compositor;
    private CanvasDevice _canvasDevice;
    private CompositionGraphicsDevice _graphicsDevice;

    private ContainerVisual _baseContainer;

    private ContainerVisual _cornerMaskContainer;
    private CompositionDrawingSurface _cornerSurface;
    private CompositionVisualSurface _cornerVisualSurface;
    private SpriteVisual _cornerMaskVisual;
    private CompositionSurfaceBrush _cornerMaskBrush;
    private CompositionMaskBrush _maskedContentBrush;
    private CompositionSurfaceBrush _sourceCornerBrush;

    private CanvasGeometry _baseOuterRect;
    private CanvasGeometry _baseInnerRect;
    private CanvasGeometry _borderShape;

    private CompositionDrawingSurface _borderSurface;
    private SpriteVisual _borderVisual;

    private ContainerVisual _innerContainer;

    private ContainerVisual _headerContainer;
    private SpriteVisual _headerRect;
    private CompositionDrawingSurface _headerDrawingSurface;
    private SpriteVisual _headerTextSprite;
    private CompositionSurfaceBrush _headerTextSurfaceBrush;

    private ContainerVisual _contentContainer;
    private SpriteVisual _contentAxisX;
    private SpriteVisual _contentAxisY;

    private ContainerVisual _adwContainer;
    private SpriteVisual _adwRect;

    private ContainerVisual _glyphBitmapContainer;
    private SpriteVisual _glyphBitmapVisual;


    private SpriteVisual _backgroundVisual;
    
    private SpriteVisual _overlayPointerEnter;
    private SpriteVisual _overlayTapped;


    private Color _backgroundColor = Color.FromArgb(0x66,0xF6,0xEA,0xCB);
    private bool _isLoaded = false;

    private float _safeCornerRadius = 0.0f;
    private float _innerCornerRadius = 0.0f;
    private float _innerBorderWidth = 0.0f;
    private float _innerBorderHeight = 0.0f;
    #endregion Private Variables

    #region Private Methods
    private void UnregisterEvents()
    {
        Tapped -= OnTapped;
        RightTapped -= OnRightTapped;
        DoubleTapped -= OnDoubleTapped;
        PointerEntered -= OnPointerEntered;
        PointerExited -= OnPointerExited;
        PointerPressed -= OnPointerPressed;
        PointerReleased -= OnPointerReleased;
        PointerMoved -= OnPointerMoved;
    }

    private void RegisterEvents()
    {
        Tapped += OnTapped;
        RightTapped += OnRightTapped;
        DoubleTapped += OnDoubleTapped;
        PointerEntered += OnPointerEntered;
        PointerExited += OnPointerExited;
        PointerPressed += OnPointerPressed;
        PointerReleased += OnPointerReleased;
        PointerMoved += OnPointerMoved;
    }

    private void CreateVisual()
    {
        _visualElement = ElementCompositionPreview.GetElementVisual(this);
        _compositor = _visualElement.Compositor;
        _canvasDevice = CanvasDevice.GetSharedDevice();
        _graphicsDevice = CanvasComposition.CreateCompositionGraphicsDevice(_compositor, _canvasDevice);

        _baseContainer = _compositor.CreateContainerVisual();

        _cornerMaskContainer = _compositor.CreateContainerVisual();
        _cornerVisualSurface = _compositor.CreateVisualSurface();
        _cornerMaskVisual = _compositor.CreateSpriteVisual();
        _maskedContentBrush = _compositor.CreateMaskBrush();
        

        _borderVisual = _compositor.CreateSpriteVisual();
        

        _innerContainer = _compositor.CreateContainerVisual();
        _backgroundVisual = _compositor.CreateSpriteVisual();
        _overlayPointerEnter = _compositor.CreateSpriteVisual();
        _overlayTapped = _compositor.CreateSpriteVisual();

        _headerContainer = _compositor.CreateContainerVisual();
        _headerRect = _compositor.CreateSpriteVisual();
        _headerTextSprite = _compositor.CreateSpriteVisual();

        _contentContainer = _compositor.CreateContainerVisual();
        _contentAxisX = _compositor.CreateSpriteVisual();
        _contentAxisY = _compositor.CreateSpriteVisual();

        _adwContainer = _compositor.CreateContainerVisual();
        _adwRect = _compositor.CreateSpriteVisual();

        _glyphBitmapContainer = _compositor.CreateContainerVisual();
        _glyphBitmapVisual = _compositor.CreateSpriteVisual();

        _baseContainer.Children.InsertAtTop(_cornerMaskVisual);
        _cornerMaskContainer.Children.InsertAtTop(_innerContainer);
        _innerContainer.Children.InsertAtTop(_backgroundVisual);

        _baseContainer.Children.InsertAtTop(_borderVisual);

        _innerContainer.Children.InsertAtTop(_contentContainer);
        _contentContainer.Children.InsertAtTop(_adwContainer);
        _adwContainer.Children.InsertAtTop(_adwRect);
        _contentContainer.Children.InsertAtTop(_contentAxisX);
        _contentContainer.Children.InsertAtTop(_contentAxisY);
        _contentContainer.Children.InsertAtTop(_glyphBitmapContainer);
        _glyphBitmapContainer.Children.InsertAtTop(_glyphBitmapVisual);

        _innerContainer.Children.InsertAtTop(_headerContainer);
        _headerContainer.Children.InsertAtTop(_headerRect);
        _headerContainer.Children.InsertAtTop(_headerTextSprite);

        _innerContainer.Children.InsertAtTop(_overlayTapped);
        _innerContainer.Children.InsertAtTop(_overlayPointerEnter);
        ElementCompositionPreview.SetElementChildVisual(this, _baseContainer);

        InitialAdvanceWidthRect();
        InitialAxis();
    }

    private void ResizeVisual(Size finalSize)
    {
        Vector2 baseSize = new Vector2((float)finalSize.Width, (float)finalSize.Height);
        float _safeCornerRadius = Math.Max(CornerRadius, BorderWidth);
        float _innerCornerRadius = Math.Max(0, _safeCornerRadius - (BorderWidth / 2));
        float _innerBorderWidth = Math.Max(0, baseSize.X - (2 * BorderWidth));
        float _innerBorderHeight = Math.Max(0, baseSize.Y - (2 * BorderWidth));

        _baseContainer.Size = baseSize;

        _baseOuterRect = CanvasGeometry.CreateRoundedRectangle(_canvasDevice, 0, 0, baseSize.X, baseSize.Y, CornerRadius, CornerRadius);
        _baseInnerRect = CanvasGeometry.CreateRoundedRectangle(_canvasDevice, BorderWidth, BorderWidth, _innerBorderWidth, _innerBorderHeight, _innerCornerRadius, _innerCornerRadius);
        _borderShape = _baseOuterRect.CombineWith(_baseInnerRect, Matrix3x2.Identity, CanvasGeometryCombine.Exclude);

        _cornerMaskContainer.Size = baseSize;
        _cornerMaskVisual.Size = baseSize;
        _cornerSurface = _graphicsDevice.CreateDrawingSurface(finalSize, DirectXPixelFormat.B8G8R8A8UIntNormalized, DirectXAlphaMode.Premultiplied);
        _cornerVisualSurface.SourceSize = baseSize;

        _borderVisual.Size = baseSize;
        _borderSurface = _graphicsDevice.CreateDrawingSurface(finalSize, DirectXPixelFormat.B8G8R8A8UIntNormalized, DirectXAlphaMode.Premultiplied);

        _innerContainer.Size = new Vector2(baseSize.X - (BorderWidth * 2), baseSize.Y - (BorderWidth * 2));
        _backgroundVisual.Size = _innerContainer.Size;
        _overlayPointerEnter.Size = _innerContainer.Size;
        _overlayTapped.Size = _innerContainer.Size;

        _headerContainer.Size = new Vector2(_innerContainer.Size.X, (float)HeaderHight);
        _headerRect.Size = _headerContainer.Size;

        _contentContainer.Size = new Vector2(_innerContainer.Size.X, (float)ContentHight);
        _contentAxisX.Size = new Vector2(_contentContainer.Size.X, 1);
        _contentAxisY.Size = new Vector2(1, _contentContainer.Size.Y);

        _glyphBitmapContainer.Size = new Vector2(Glyph.BitmapWidth * Zoom, Glyph.BitmapHeight * Zoom);

        float adw = Glyph.AdvanceWidth > 0 ? Glyph.AdvanceWidth : 0;
        _adwContainer.Size = new Vector2(adw * Zoom, (float)ContentHight);
        _adwRect.Size = _adwContainer.Size;
    }

    private void ArrangeVisual()
    {
        Vector3 baseContainerOffset = Vector3.Zero;

        _baseContainer.Offset = baseContainerOffset;
        _cornerMaskContainer.Offset = baseContainerOffset;
        _cornerMaskVisual.Offset = baseContainerOffset;
        _cornerVisualSurface.SourceOffset = Vector2.Zero;
        _borderVisual.Offset = baseContainerOffset;
        
        _innerContainer.Offset = new Vector3(BorderWidth, BorderWidth, 0);
        _backgroundVisual.Offset = Vector3.Zero;
        _overlayPointerEnter.Offset = Vector3.Zero;
        _overlayTapped.Offset = Vector3.Zero;

        _headerContainer.Offset = Vector3.Zero;
        _headerRect.Offset = Vector3.Zero;

        _contentContainer.Offset = new Vector3(0, (float)HeaderHight, 0);
        _contentAxisX.Offset = new Vector3(0, Axis_X, 0);
        _contentAxisY.Offset = new Vector3(Axis_Y, 0, 0);

        _adwContainer.Offset = new Vector3(Axis_Y, 0, 0);
        _adwRect.Offset = Vector3.Zero;

        float bitmapX = Axis_Y + (Glyph.OffsetX * Zoom);
        float bitmapY = Axis_X - ((Glyph.BitmapHeight + Glyph.OffsetY) * Zoom);
        _glyphBitmapContainer.Offset = new Vector3(bitmapX, bitmapY, 0);
    }

    private void RenderVisual()
    {
        CreateBaseContainerCornerMask();
        CreateBaseContinaerBorder();
        InitialInnerContainerBackground();
        InitialOverlayPointerEnter();
        InitialOverlayTapped();
        InitialHeader();
        InitialGlyphBitmap();
    }

    private void CreateBaseContainerCornerMask()
    {
        using (CanvasDrawingSession ds = CanvasComposition.CreateDrawingSession(_cornerSurface))
        {
            ds.Clear(Colors.Transparent);
            ds.FillGeometry(_baseInnerRect, Colors.White);
            ds.Antialiasing = CanvasAntialiasing.Antialiased;
        }
        _cornerMaskBrush = _compositor.CreateSurfaceBrush(_cornerSurface);
        _maskedContentBrush.Mask = _cornerMaskBrush;
        _cornerMaskVisual.Brush = _maskedContentBrush;
        _cornerVisualSurface.SourceVisual = _cornerMaskContainer;
        _sourceCornerBrush = _compositor.CreateSurfaceBrush(_cornerVisualSurface);
        _maskedContentBrush.Source = _sourceCornerBrush;
    }

    private void CreateBaseContinaerBorder()
    {
        if (BorderWidth <= 0)
            return;
        using (CanvasDrawingSession ds = CanvasComposition.CreateDrawingSession(_borderSurface))
        {
            ds.Clear(Colors.Transparent);
            ds.FillGeometry(_borderShape, BorderColor);
            ds.Antialiasing = CanvasAntialiasing.Antialiased;
        }
        _borderVisual.Brush = _compositor.CreateSurfaceBrush(_borderSurface);
    }

    private void InitialInnerContainerBackground()
    {
        _backgroundVisual.Brush = _compositor.CreateColorBrush(_backgroundColor);
    }

    private void InitialOverlayPointerEnter()
    {
        _overlayPointerEnter.Brush = _compositor.CreateColorBrush(Colors.Transparent);
    }

    private void InitialOverlayTapped()
    {
        _overlayTapped.Brush = _compositor.CreateColorBrush(Colors.Transparent);
    }

    private void InitialHeader()
    {
        
        _headerRect.Brush = _compositor.CreateColorBrush(Colors.LightSteelBlue);

        float widthPadding = (float)Math.Ceiling(HeaderPadding.Left + HeaderPadding.Right);
        float heightPadding = (float)Math.Ceiling(HeaderPadding.Top + HeaderPadding.Bottom);
        var truncatedText = TruncateText(string.IsNullOrEmpty(Glyph.Name)? $"Glyph {Glyph.Index}": Glyph.Name, HeaderFontSize, _headerContainer.Size.X - widthPadding);
        var textSize = MeasureText(truncatedText, HeaderFontSize, (float)(_headerContainer.Size.X - widthPadding));
        float textLeft = (float)((_headerContainer.Size.X - (float)textSize.Width) / 2.0f);
        float textTop = (float)Math.Floor(heightPadding / 2.0);

        _headerDrawingSurface = _graphicsDevice.CreateDrawingSurface(textSize,DirectXPixelFormat.B8G8R8A8UIntNormalized,DirectXAlphaMode.Premultiplied);

        using (CanvasDrawingSession ds = CanvasComposition.CreateDrawingSession(_headerDrawingSurface))
        {
            ds.Clear(Colors.Transparent);
            ds.TextAntialiasing = CanvasTextAntialiasing.Auto;
            ds.DrawText(
                truncatedText,
                Vector2.Zero,
                Colors.Black,
                new CanvasTextFormat()
                {
                    FontSize = HeaderFontSize,
                    HorizontalAlignment = CanvasHorizontalAlignment.Left,
                }
            );
        }
 
        _headerTextSprite.Size = new Vector2((float)textSize.Width, (float)textSize.Height);
        _headerTextSprite.Offset = new Vector3(textLeft, textTop, 0);
        _headerTextSurfaceBrush = _compositor.CreateSurfaceBrush(_headerDrawingSurface);
        _headerTextSprite.Brush = _headerTextSurfaceBrush;
        _headerTextSprite.IsPixelSnappingEnabled = true;
        
    }

    private void InitialAxis()
    {
        _contentAxisX.Brush = _compositor.CreateColorBrush(Colors.IndianRed);
        _contentAxisX.Opacity = 0.25f;
        _contentAxisY.Brush = _compositor.CreateColorBrush(Colors.IndianRed);
        _contentAxisY.Opacity = 0.25f;
    }

    private void InitialAdvanceWidthRect()
    {
        _adwRect.Brush = _compositor.CreateColorBrush(Color.FromArgb(0x88,0xFF,0xE3,0xE3));
    }

    private void InitialGlyphBitmap()
    {

        Size bitmapSize = new Size(_glyphBitmapContainer.Size.X == 0 ? 1 : _glyphBitmapContainer.Size.X, _glyphBitmapContainer.Size.Y == 0 ? 1 : _glyphBitmapContainer.Size.Y);
        
            CompositionDrawingSurface surface = _graphicsDevice.CreateDrawingSurface(bitmapSize, DirectXPixelFormat.B8G8R8A8UIntNormalized, DirectXAlphaMode.Premultiplied);
            int width = Glyph.BitmapWidth * Zoom;
            int height = Glyph.BitmapHeight * Zoom;
            using (var ds = CanvasComposition.CreateDrawingSession(surface))
            {
                ds.Clear(Colors.Transparent);
                for (int y = 0; y < Glyph.BitmapHeight; y++)
                {
                    for (int x = 0; x < Glyph.BitmapWidth; x++)
                    {
                        byte value = GetPixel(Glyph.Bitmap, Glyph.BitmapWidth, Glyph.BitsPerPixel, x, y);
                        if (value > 0)
                        {
                            var color = Color.FromArgb((byte)(value * 255 / ((1 << Glyph.BitsPerPixel) - 1)), 0, 0, 0);
                            var rect = new Rect(x * Zoom, y * Zoom, Zoom, Zoom);
                            ds.FillRectangle(rect, color);
                        }
                        else
                        {
                            var color = Color.FromArgb(0, 0, 0, 0);
                            var rect = new Rect(x * Zoom, y * Zoom, Zoom, Zoom);
                            ds.FillRectangle(rect, color);
                        }
                    }
                }
            }

            var brush = _compositor.CreateSurfaceBrush(surface);

            _glyphBitmapVisual.Brush = brush;
            _glyphBitmapVisual.Size = new Vector2(width, height);

        
    }

    private Size MeasureText(string text, float fontSize, float maxWidth)
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
            text,
            textFormat,
            maxWidth,
            256
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
        //_visualElement.Dispose();
        //_compositor.Dispose();
        //_canvasDevice.Dispose();
        //_graphicsDevice.Dispose();

        //_baseContainer.Dispose();

        //_cornerMaskContainer.Dispose();
        //_cornerVisualSurface.Dispose();
        //_cornerMaskVisual.Dispose();
        //_maskedContentBrush.Dispose();


        //_borderVisual.Dispose();


        //_innerContainer.Dispose();
        //_backgroundVisual.Dispose();
        //_overlayPointerEnter.Dispose();
        //_overlayTapped.Dispose();

        //_headerContainer.Dispose();
        //_headerRect.Dispose();
        //_headerTextSprite.Dispose();

        //_contentContainer.Dispose();
        //_contentAxisX.Dispose();
        //_contentAxisY.Dispose();

        //_adwContainer.Dispose();
        //_adwRect.Dispose();

        //_glyphBitmapContainer.Dispose();
        //_glyphBitmapVisual.Dispose();

    }
    #endregion Private Methods

    #region Public Variables
    public double HeaderHight { get; set; }
    public double ContentHight { get; set; }
    public float Axis_X { get; set; }
    public float Axis_Y { get; set; }
    public string ToolTipText
    {
        get => (string)ToolTipService.GetToolTip(this);
        set => ToolTipService.SetToolTip(this, value);
    }
    #endregion Public Variables
}