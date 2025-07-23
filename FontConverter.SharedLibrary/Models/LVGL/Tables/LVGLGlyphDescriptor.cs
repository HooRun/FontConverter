namespace FontConverter.SharedLibrary.Models;

public class LVGLGlyphDescriptor
{
    public LVGLGlyphDescriptor()
    {
        Width = 0;
        Height = 0;
        OffsetX = 0;
        OffsetY = 0;
        AdvanceWidth = 0;
        BitmapIndex = 0;
        OriginalWidth = 0;
    }

    public LVGLGlyphDescriptor(int glyphIndex, int width, int height, int offsetX, int offsetY, int advanceWidth) : this()
    {
        Width = width;
        Height = height;
        OffsetX = offsetX;
        OffsetY = offsetY;
        AdvanceWidth = advanceWidth;
        OriginalWidth = Width;
    }

    
    
    public int Width { get; set; }
    public int Height { get; set; }
    public int OffsetX { get; set; }
    public int OffsetY { get; set; }
    public int AdvanceWidth { get; set; }
    public int BitmapIndex { get; set; }
    public int OriginalWidth { get; set; }
}
