using LVGLFontConverter.Library.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LVGLFontConverter.Library.Models;

public class LVGLFontGlyph
{
    public LVGLFontGlyph()
    {
        Name = string.Empty;
        Description = string.Empty;
        Bitmap = [];
        Unicodes = [];

        IsEmpty = false;
        IsUnMapped = false;
        IsMultiMapped = false;

    }

    public string Name { get; set; }
    public string Description { get; set; }

    public int Index { get; set; }
    public int AdvanceWidth { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int OffsetX { get; set; }
    public int OffsetY { get; set; }

    public int LineHeight { get; set; }
    public int BaseLine { get; set; }
    public int MaxCharWidth { get; set; }
    public int YAxisPosition { get; set; }

    public byte BitsPerPixel { get; set; }
    public int Threshold {  get; set; }
    public int Gamma { get; set; }

    public byte[] Bitmap { get; set; }
    public int BitmapSize { get; set; }
    public int BitmapWidth { get; set; }
    public int BitmapHeight { get; set; }
    
    public List<(uint CodePoint, string Name)> Unicodes { get; set; }

    public bool IsEmpty { get; set; }
    public bool IsUnMapped { get; set; }
    public bool IsSingleMapped { get; set; }
    public bool IsMultiMapped { get; set; }

}
