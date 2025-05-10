using Microsoft.UI.Xaml.Markup;
using Microsoft.Windows.ApplicationModel.Resources;

namespace LVGLFontConverter.Helpers;

public class LocExtension : MarkupExtension
{
    public string Key { get; set; } = string.Empty;

    protected override object ProvideValue()
    {
        if (string.IsNullOrWhiteSpace(Key))
            return string.Empty;

        var loader = new ResourceLoader();
        return loader.GetString(Key);
    }
}