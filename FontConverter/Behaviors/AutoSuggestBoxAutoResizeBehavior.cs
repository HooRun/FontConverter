using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml;
using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using System.Collections;

namespace LVGLFontConverter.Behaviors;

public class AutoSuggestBoxAutoResizeBehavior : Behavior<AutoSuggestBox>
{
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.GotFocus += OnAutoSuggestBoxFocus;
        AssociatedObject.TextChanged += OnAutoSuggestBoxFocus;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.GotFocus -= OnAutoSuggestBoxFocus;
        AssociatedObject.TextChanged -= OnAutoSuggestBoxFocus;
    }

    private void OnAutoSuggestBoxFocus(object sender, object e)
    {
        AssociatedObject.DispatcherQueue.TryEnqueue(async () =>
        {
            await Task.Delay(50);

            var popup = FindPopup(AssociatedObject);
            if (popup is not null && popup.Child is FrameworkElement child)
            {
                double maxItemWidth = GetWidestItemWidth();
                child.MinWidth = maxItemWidth + 32;
                child.MaxWidth = maxItemWidth + 32;
            }
        });
    }

    private double GetWidestItemWidth()
    {
        double max = 0;
        var items = AssociatedObject.ItemsSource;
        if (items == null) return 0;

        foreach (var item in (IEnumerable)items)
        {
            var text = item?.ToString();
            if (string.IsNullOrEmpty(text)) continue;

            var tb = new TextBlock
            {
                Text = text,
                FontFamily = AssociatedObject.FontFamily,
                FontSize = AssociatedObject.FontSize
            };

            tb.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            max = Math.Max(max, tb.DesiredSize.Width);
        }

        return max;
    }

    private Popup? FindPopup(DependencyObject root)
    {
        int count = VisualTreeHelper.GetChildrenCount(root);
        for (int i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(root, i);
            if (child is Popup popup && popup.Name == "SuggestionsPopup")
                return popup;

            var result = FindPopup(child);
            if (result != null) return result;
        }

        return null;
    }
}