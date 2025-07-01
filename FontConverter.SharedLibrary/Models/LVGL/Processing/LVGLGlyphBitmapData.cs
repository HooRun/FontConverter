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
        SVG = new();
    }

    public LVGLGlyphBitmapData(int glyphIndex, byte[] bitmap, SKRectI bounds, LVGLGlyphSVG svg) : this()
    {
        Index = glyphIndex;
        Bitmap = bitmap;
        Bounds = bounds;
        SVG = svg;
    }

    public int Index { get; set; }
    public byte[] Bitmap { get; set; }
    public SKRectI Bounds { get; set; }
    public LVGLGlyphSVG SVG { get; set; }
}
