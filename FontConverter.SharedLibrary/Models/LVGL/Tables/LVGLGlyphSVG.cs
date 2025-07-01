using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FontConverter.SharedLibrary.Models;

public class LVGLGlyphSVG
{
    public LVGLGlyphSVG()
    {
        Width = 0.0f;
        Height = 0.0f;
        TranslateX = 0.0f;
        TranslateY = 0.0f;
        Path = string.Empty;
    }

    public LVGLGlyphSVG(float width, float height, float translateX, float translateY, string path) : this()
    {
        Width = width;
        Height = height;
        TranslateX = translateX;
        TranslateY = translateY;
        Path = path;
    }

    public float Width { get; set; }
    public float Height { get; set; }
    public float TranslateX { get; set; }
    public float TranslateY { get; set; }
    public string Path { get; set; } 
    
}
