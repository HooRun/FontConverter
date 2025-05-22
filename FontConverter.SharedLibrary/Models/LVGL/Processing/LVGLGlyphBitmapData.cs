using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FontConverter.SharedLibrary.Models;

public class LVGLGlyphBitmapData
{
    public LVGLGlyphBitmapData()
    {
        Index = -1;
        Bitmap = [];
        Bounds = new();
    }

    public LVGLGlyphBitmapData(int glyphIndex, byte[] bitmap, SKRectI bounds) : this()
    {
        Index = glyphIndex;
        Bitmap = bitmap;
        Bounds = bounds;
    }

    public int Index { get; set; }
    public byte[] Bitmap { get; set; }
    public SKRectI Bounds { get; set; }
}
