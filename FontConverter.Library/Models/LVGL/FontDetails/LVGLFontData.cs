using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LVGLFontConverter.Library.Models;

public class LVGLFontData
{
    public LVGLFontData()
    {
        FullFontName = string.Empty;
        FontFamily = string.Empty;
        FontSubfamily = string.Empty;
        Manufacturer = string.Empty;
        FontRevision = 0.0;
        Created = new DateTime();
        Modified = new DateTime();
        AdvanceWidthMax = 0;
        Ascent = 0;
        Descent = 0;
        XMin = 0;
        YMin = 0;
        XMax = 0;
        YMax = 0;
        MaxCharWidth = 0;
    }

    public string FullFontName { get; set; }
    public string FontFamily { get; set; }
    public string FontSubfamily { get; set; }
    public string Manufacturer { get; set; }
    public double FontRevision { get; set; }
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
    public int AdvanceWidthMax { get; set; }
    public int Ascent { get; set; }
    public int Descent { get; set; }
    public int XMin { get; set; }
    public int YMin { get; set; }
    public int XMax { get; set; }
    public int YMax { get; set; }
    public int MaxCharWidth { get; set; }

}
