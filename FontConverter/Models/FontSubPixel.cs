using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LVGLFontConverter.Library.Helpers.LVGLFontEnums;

namespace LVGLFontConverter.Models;

public class FontSubPixel
{
    public SUB_Pixel_ENUM SubPixel { get; set; }
    public string Description { get; set; } = string.Empty;
}
