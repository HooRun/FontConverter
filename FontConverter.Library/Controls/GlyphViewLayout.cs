using Microsoft.UI.Xaml.Controls;

namespace LVGLFontConverter.Library;

public class GlyphViewLayout : UniformGridLayout
{
    public GlyphViewLayout()
    {
        MinItemWidth = 50;
        MinItemHeight = 80;
        MinRowSpacing = 2;
        MinColumnSpacing = 2;
        MaximumRowsOrColumns = -1;
        ItemsStretch = UniformGridLayoutItemsStretch.Uniform;
        
    }

}
