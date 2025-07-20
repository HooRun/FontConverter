using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FontConverter.SharedLibrary.Helpers;

public static class InitialSubPixelListHelper
{
    private static readonly SortedList<LVGLFontEnums.SUB_Pixel_ENUM, string> _SubPixelList = new()
    {
        { LVGLFontEnums.SUB_Pixel_ENUM.SUB_PIXEL_NONE, "None" },
        { LVGLFontEnums.SUB_Pixel_ENUM.SUB_PIXEL_Horizontal, "Horizontal" },
        { LVGLFontEnums.SUB_Pixel_ENUM.SUB_PIXEL_Vertical, "Vertical" },
        { LVGLFontEnums.SUB_Pixel_ENUM.SUB_PIXEL_Both, "Both" }
    };

    public static SortedList<LVGLFontEnums.SUB_Pixel_ENUM, string> InitialSubPixellList()
    {
        return _SubPixelList;
    }

    
}
