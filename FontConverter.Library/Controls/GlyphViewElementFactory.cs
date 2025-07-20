using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LVGLFontConverter.Library;

public class GlyphViewElementFactory : IElementFactory
{
    private readonly Stack<GlyphViewItem> _pool = new();


    public UIElement GetElement(ElementFactoryGetArgs args)
    {
        GlyphViewItem element;

        if (_pool.Count > 0)
        {
            element = _pool.Pop();
        }
        else
        {
            element = new GlyphViewItem();
        }

        element.Glyph = (Models.LVGLFontGlyph)args.Data;
        return element;
    }

    public void RecycleElement(ElementFactoryRecycleArgs args)
    {
        if (args.Element is GlyphViewItem item)
        {
            DisposeItem(item);
            _pool.Push(item);
        }
    }

    private void DisposeItem(GlyphViewItem item)
    {
        item.Dispose();
        //item.ClearValue(GlyphViewItem.GlyphProperty);
    }
}
