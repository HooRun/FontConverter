using FontConverter.Blazor.Models.GlyphsView;

namespace FontConverter.Blazor.EventsArgs;

public class LastSelectedGlyphEventArgs : EventArgs
{
    public LastSelectedGlyphEventArgs()
    {
        Glyph = new();
        Selected = false;
    }

    public LastSelectedGlyphEventArgs(GlyphItemModel glyph, bool selected) : this()
    {
        Glyph = glyph;
        Selected = selected;
    }

    public GlyphItemModel Glyph { get; set; }
    public bool Selected { get; set; }
}
