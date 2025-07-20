using FontConverter.SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FontConverter.SharedLibrary.Helpers;

public static class InitialBitPerPixelListHelper
{
    private static readonly SortedList<LVGLFontEnums.BIT_PER_PIXEL_ENUM, string> _BitPerPixelList = new()
    {
        { LVGLFontEnums.BIT_PER_PIXEL_ENUM.BPP_1, "1 Bit Per Pixel" },
        { LVGLFontEnums.BIT_PER_PIXEL_ENUM.BPP_2, "2 Bit Per Pixel" },
        { LVGLFontEnums.BIT_PER_PIXEL_ENUM.BPP_4, "4 Bit Per Pixel" },
        { LVGLFontEnums.BIT_PER_PIXEL_ENUM.BPP_8, "8 Bit Per Pixel" },
    };

    public static SortedList<LVGLFontEnums.BIT_PER_PIXEL_ENUM, string> InitialBitPerPixelList()
    {
        return _BitPerPixelList;
    }
}
