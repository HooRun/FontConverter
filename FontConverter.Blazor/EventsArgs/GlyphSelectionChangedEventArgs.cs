namespace FontConverter.Blazor.EventsArgs;

public class GlyphSelectionChangedEventArgs : EventArgs
{
    public GlyphSelectionChangedEventArgs()
    {
        GlyphID = -1;
        Selected = false;
    }

    public GlyphSelectionChangedEventArgs(int glyphID, bool selected) : this()
    {
        GlyphID = glyphID;
        Selected = selected;
    }

    public int GlyphID { get; set; }
    public bool Selected { get; set; }

}
