using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FontConverter.SharedLibrary.Data;



public record GlyphToBitmapResult
{
    public int Index { get; set; }
    public byte[] Bitmap { get; set; }
    public SKRectI Bounds { get; set; }

    public GlyphToBitmapResult(int glyphIndex, byte[] bitmap, SKRectI bounds)
    {
        Index = glyphIndex;
        Bitmap = bitmap;
        Bounds = bounds;
    }
}