using LVGLFontConverter.Library.Models;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.UI;
using Windows.Foundation;

namespace LVGLFontConverter.Library;

[TemplatePart(Name = PART_ScrollViewer, Type = typeof(ScrollViewer))]
[TemplatePart(Name = PART_ItemsRepeater, Type = typeof(ItemsRepeater))]
public class GlyphView : Control
{
    #region Constructor
    public GlyphView()
    {
        DefaultStyleKey = typeof(GlyphView);
    }
    #endregion

    #region Internal Events
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _scrollViewer = GetTemplateChild(PART_ScrollViewer) as ScrollViewer;
        _repeater = GetTemplateChild(PART_ItemsRepeater) as ItemsRepeater;

        if (_scrollViewer!=null && _repeater != null)
        {
            _scrollViewer.HorizontalScrollMode = ScrollMode.Disabled;
            _scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            _scrollViewer.VerticalScrollMode = ScrollMode.Enabled;
            _scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

            CalculateGlyphsSize();
            GlyphViewLayout.MinItemWidth = _glyphWidth;
            GlyphViewLayout.MinItemHeight = _glyphHeight;

            _repeater.Layout = GlyphViewLayout;
            _repeater.ItemTemplate = GlyphViewFactory;
            _repeater.ItemsSource = LVFont;
            _repeater.HorizontalCacheLength = 0;
            _repeater.VerticalCacheLength = 2;
            _repeater.Margin = new Thickness(0, 0, 15, 0);

            _repeater.ElementPrepared += _repeater_ElementPrepared;

            Loaded += OnLoaded;
        }        
    }

    private void _repeater_ElementPrepared(ItemsRepeater sender, ItemsRepeaterElementPreparedEventArgs args)
    {
        if (args.Element is GlyphViewItem glyphViewItem && glyphViewItem!=null)
        {
            glyphViewItem.Width = _glyphWidth;
            glyphViewItem.Height = _glyphHeight;
            glyphViewItem.HeaderHight = _glyphHeaderHeight;
            glyphViewItem.ContentHight = _glyphContentHeight;
            glyphViewItem.Axis_X = _axis_X;
            glyphViewItem.Axis_Y = _axis_Y;
            glyphViewItem.Zoom = GlyphZoom;
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _isLoaded = true;
    }

    #endregion Internal Events

    #region Dependency Properties
    public LVGLFont LVFont
    {
        get { return (LVGLFont)GetValue(LVFontProperty); }
        set { 
            SetValue(LVFontProperty, value); }
    }
    public static readonly DependencyProperty LVFontProperty =
        DependencyProperty.Register("LVFont", typeof(LVGLFont), typeof(GlyphView), new PropertyMetadata(new LVGLFont(), OnLVFontChanged));
    private static void OnLVFontChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue.Equals(e.OldValue))
            return;
        if (d is GlyphView glyphView && glyphView is not null)
        {
            if (glyphView._repeater is not null)
            {
                glyphView.CalculateGlyphsSize();
                glyphView._repeater.ItemsSource = e.NewValue as LVGLFont;
                glyphView._repeater.InvalidateMeasure();
                glyphView._repeater.InvalidateArrange();
                glyphView._repeater.UpdateLayout();
                glyphView.UpdateLayout();
            }
        }
    }

    public double Spacing
    {
        get { return (double)GetValue(SpacingProperty); }
        set { SetValue(SpacingProperty, value); }
    }
    public static readonly DependencyProperty SpacingProperty =
        DependencyProperty.Register("Spacing", typeof(double), typeof(GlyphView), new PropertyMetadata(5.0));

    public double GlyphMinWidth
    {
        get { return (double)GetValue(GlyphMinWidthProperty); }
        set { SetValue(GlyphMinWidthProperty, value); }
    }
    public static readonly DependencyProperty GlyphMinWidthProperty =
        DependencyProperty.Register("GlyphMinWidth", typeof(double), typeof(GlyphView), new PropertyMetadata(0.0));

    public int GlyphZoom
    {
        get { return (int)GetValue(GlyphZoomProperty); }
        set
        {
            if (value < 1) value = 1;
            if (value > 100) value = 100;
            SetValue(GlyphZoomProperty, value);
        }
    }
    public static readonly DependencyProperty GlyphZoomProperty =
        DependencyProperty.Register("GlyphZoom", typeof(int), typeof(GlyphView), new PropertyMetadata(1, OnZoomChanged));
    private static void OnZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is GlyphView glyphView && glyphView is not null)
        {

        }
    }

    public float GlyphCornerRadius
    {
        get { return (float)GetValue(GlyphCornerRadiusProperty); }
        set { SetValue(GlyphCornerRadiusProperty, value); }
    }
    public static readonly DependencyProperty GlyphCornerRadiusProperty =
        DependencyProperty.Register("GlyphCornerRadius", typeof(float), typeof(GlyphView), new PropertyMetadata(0.0f));

    public float GlyphBorderWidth
    {
        get { return (float)GetValue(GlyphBorderWidthProperty); }
        set { SetValue(GlyphBorderWidthProperty, value); }
    }
    public static readonly DependencyProperty GlyphBorderWidthProperty =
        DependencyProperty.Register("GlyphBorderWidth", typeof(float), typeof(GlyphView), new PropertyMetadata(2.0f));

    public Color GlyphBorderColor
    {
        get { return (Color)GetValue(GlyphBorderColorProperty); }
        set { SetValue(GlyphBorderColorProperty, value); }
    }
    public static readonly DependencyProperty GlyphBorderColorProperty =
        DependencyProperty.Register("GlyphBorderColor", typeof(Color), typeof(GlyphView), new PropertyMetadata(Colors.Transparent));

    public Color GlyphHoverColor
    {
        get { return (Color)GetValue(GlyphHoverColorProperty); }
        set { SetValue(GlyphHoverColorProperty, value); }
    }
    public static readonly DependencyProperty GlyphHoverColorProperty =
        DependencyProperty.Register("GlyphHoverColor", typeof(Color), typeof(GlyphView), new PropertyMetadata(Color.FromArgb(15, 0, 0, 30)));

    public Color GlyphSelectedColor
    {
        get { return (Color)GetValue(GlyphSelectedColorProperty); }
        set { SetValue(GlyphSelectedColorProperty, value); }
    }
    public static readonly DependencyProperty GlyphSelectedColorProperty =
        DependencyProperty.Register("GlyphSelectedColor", typeof(Color), typeof(GlyphView), new PropertyMetadata(Color.FromArgb(15, 0, 0, 30)));

    public float GlyphHeaderFontSize
    {
        get { return (float)GetValue(GlyphHeaderFontSizeProperty); }
        set { SetValue(GlyphHeaderFontSizeProperty, value); }
    }
    public static readonly DependencyProperty GlyphHeaderFontSizeProperty =
        DependencyProperty.Register("GlyphHeaderFontSize", typeof(float), typeof(GlyphView), new PropertyMetadata(12.0f));

    public Color GlyphHeaderBackground
    {
        get { return (Color)GetValue(GlyphHeaderBackgroundProperty); }
        set { SetValue(GlyphHeaderBackgroundProperty, value); }
    }
    public static readonly DependencyProperty GlyphHeaderBackgroundProperty =
        DependencyProperty.Register("GlyphHeaderBackground", typeof(Color), typeof(GlyphView), new PropertyMetadata(Colors.LightSteelBlue));

    public Color GlyphHeaderForeground
    {
        get { return (Color)GetValue(GlyphHeaderForegroundProperty); }
        set { SetValue(GlyphHeaderForegroundProperty, value); }
    }
    public static readonly DependencyProperty GlyphHeaderForegroundProperty =
        DependencyProperty.Register("GlyphHeaderForeground", typeof(Color), typeof(GlyphView), new PropertyMetadata(Colors.Black));

    public Thickness GlyphHeaderPadding
    {
        get { return (Thickness)GetValue(GlyphHeaderPaddingProperty); }
        set { SetValue(GlyphHeaderPaddingProperty, value); }
    }
    public static readonly DependencyProperty GlyphHeaderPaddingProperty =
        DependencyProperty.Register("GlyphHeaderPadding", typeof(Thickness), typeof(GlyphView), new PropertyMetadata(new Thickness(2)));

    public Thickness GlyphContentPadding
    {
        get { return (Thickness)GetValue(GlyphContentPaddingProperty); }
        set { SetValue(GlyphContentPaddingProperty, value); }
    }
    public static readonly DependencyProperty GlyphContentPaddingProperty =
        DependencyProperty.Register("GlyphContentPadding", typeof(Thickness), typeof(GlyphViewItem), new PropertyMetadata(new Thickness(5.0)));

    public Color GlyphBackground
    {
        get { return (Color)GetValue(GlyphBackgroundProperty); }
        set { SetValue(GlyphBackgroundProperty, value); }
    }
    public static readonly DependencyProperty GlyphBackgroundProperty =
        DependencyProperty.Register("GlyphBackground", typeof(Color), typeof(GlyphView), new PropertyMetadata(Colors.WhiteSmoke));

    public Color GlyphForeground
    {
        get { return (Color)GetValue(GlyphForegroundProperty); }
        set { SetValue(GlyphForegroundProperty, value); }
    }
    public static readonly DependencyProperty GlyphForegroundProperty =
        DependencyProperty.Register("Foreground", typeof(Color), typeof(GlyphView), new PropertyMetadata(Colors.Black));

    #endregion Dependency Properties

    #region Event Handlers

    #endregion Event Handlers

    #region Private Variables
    private const string PART_ScrollViewer = "PART_ScrollViewer";
    private const string PART_ItemsRepeater = "PART_ItemsRepeater";
    private ScrollViewer _scrollViewer;
    public ItemsRepeater _repeater { get; set; }

    private bool _isLoaded = false;
    private double _glyphWidth = 0.0;
    private double _glyphHeight = 0.0;
    private double _glyphHeaderHeight = 0.0;
    private double _glyphContentHeight = 0.0;
    private float _axis_X = 0.0f;
    private float _axis_Y = 0.0f;
    #endregion Private Variables

    #region Private Methods
    public void CalculateGlyphsSize()
    {
        double contentWidthPadding = GlyphContentPadding.Left + GlyphContentPadding.Right;
        double contentHeightPadding = GlyphContentPadding.Top + GlyphContentPadding.Bottom;
        double borderWith = (GlyphBorderWidth * 2);

        double glyphWidth = (LVFont.FontData.MaxCharWidth * GlyphZoom) + contentWidthPadding + borderWith;
        double heightRatio = glyphWidth < GlyphMinWidth ? GlyphMinWidth / glyphWidth : 1.0;
        if (heightRatio != 1.0)
        {
            GlyphZoom *= (int)Math.Ceiling(heightRatio);
            glyphWidth = (LVFont.FontData.MaxCharWidth * GlyphZoom) + contentWidthPadding + borderWith;
        }

        _glyphWidth = glyphWidth;
        
        Size headerFontSize = MeasureText("HQ", GlyphHeaderFontSize, (float)_glyphWidth);
        _glyphHeaderHeight = headerFontSize.Height + (GlyphHeaderPadding.Bottom + GlyphHeaderPadding.Top);

        _glyphContentHeight = (LVFont.FontProperties.LineHeight * GlyphZoom) + contentHeightPadding;

        _glyphHeight = _glyphHeaderHeight + _glyphContentHeight + borderWith;

        _axis_X = (float)_glyphContentHeight - (LVFont.FontProperties.BaseLine * GlyphZoom) - (float)GlyphContentPadding.Bottom;
        if (LVFont.FontData.XMin>0)
        {
            _axis_Y = (float)GlyphContentPadding.Left;
        }
        else
        {
            _axis_Y = (-LVFont.FontData.XMin * GlyphZoom) + (float)GlyphContentPadding.Left;
        }
        GlyphViewLayout.MinItemWidth = _glyphWidth;
        GlyphViewLayout.MinItemHeight = _glyphHeight;
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
        using var textLayout = new CanvasTextLayout( device, text, textFormat, maxWidth, 256);
        Size response = new Size((float)Math.Ceiling(textLayout.LayoutBounds.Width), (float)Math.Ceiling(textLayout.LayoutBounds.Height));
        return response;
    }

    #endregion Private Methods

    #region Public Variables
    public GlyphViewElementFactory GlyphViewFactory { get; } = new();
    public GlyphViewLayout GlyphViewLayout { get; set; } = new();
    #endregion Public Variables

    #region Public Methods

    #endregion Public Methods

}