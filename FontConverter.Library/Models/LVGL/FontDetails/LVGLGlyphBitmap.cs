using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LVGLFontConverter.Library.Models;

public class LVGLGlyphBitmap
{
    public LVGLGlyphBitmap()
    {
        
    }

    public LVGLGlyphBitmap(byte[] bitmap, int bitmapWidth, int bitmapHeight) : this()
    {
        Bitmap = bitmap;
        BitmapWidth = bitmapWidth;
        BitmapHeight = bitmapHeight;
    }

    public byte[] Bitmap { get; set; } = [];
    public int BitmapWidth { get; set; }
    public int BitmapHeight { get; set; }
}
