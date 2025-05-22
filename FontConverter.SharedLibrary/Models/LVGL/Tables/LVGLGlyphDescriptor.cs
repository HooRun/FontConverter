namespace FontConverter.SharedLibrary.Models;

public class LVGLGlyphDescriptor
{
    public LVGLGlyphDescriptor()
    {
        BitmapIndex = 0;
        AdvanceWidth = 0;
        Width = 0;
        Height = 0;
        OffsetX = 0;
        OffsetY = 0;
    }

    public LVGLGlyphDescriptor(int glyphIndex, int bitmapIndex, int advanceWidth, int width, int height, int offsetX, int offsetY) : this()
    {
        BitmapIndex = bitmapIndex;
        AdvanceWidth = advanceWidth;
        Width = width;
        Height = height;
        OffsetX = offsetX;
        OffsetY = offsetY;
    }

    public int BitmapIndex { get; set; }
    public int AdvanceWidth { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int OffsetX { get; set; }
    public int OffsetY { get; set; }
}
