using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FontConverter.SharedLibrary.Models;

public class LVGLGlyphViewItemProperties
{
    public LVGLGlyphViewItemProperties()
    {
        XMin = 0;
        BaseLine = 0;
        ItemWidth = 0;
        ItemHeight = 0;
        Zoom = 1;
    }

    public int XMin { get; set; }
    public int BaseLine { get; set; }
    public int ItemWidth { get; set; }
    public int ItemHeight { get; set; }
    public int Zoom { get; set; }
}
