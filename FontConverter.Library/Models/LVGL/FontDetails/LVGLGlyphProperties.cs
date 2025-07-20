using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LVGLFontConverter.Library.Models;

public class LVGLGlyphProperties
{
    public LVGLGlyphProperties()
    {
        
    }

    public LVGLGlyphProperties(string name, string description, int advanceWidth, int width, int height, int offsetX, int offsetY) : this()
    {
        Name = name;
        Description = description;
        AdvanceWidth = advanceWidth;
        Width = width;
        Height = height;
        OffsetX = offsetX;
        OffsetY = offsetY;
    }

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int AdvanceWidth { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int OffsetX { get; set; }
    public int OffsetY { get; set; }
}
