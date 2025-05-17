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
    public static async Task<SortedList<LVGLFontEnums.BIT_PER_PIXEL_ENUM, string>> InitialBitPerPixelList(CancellationToken cancellationToken = default)
    {
        SortedList<LVGLFontEnums.BIT_PER_PIXEL_ENUM, string> bitPerPixelList = new();
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                bitPerPixelList.Add(LVGLFontEnums.BIT_PER_PIXEL_ENUM.BPP_1, "1 Bit Per Pixel");
                bitPerPixelList.Add(LVGLFontEnums.BIT_PER_PIXEL_ENUM.BPP_2, "2 Bit Per Pixel");
                bitPerPixelList.Add(LVGLFontEnums.BIT_PER_PIXEL_ENUM.BPP_4, "4 Bit Per Pixel");
                bitPerPixelList.Add(LVGLFontEnums.BIT_PER_PIXEL_ENUM.BPP_8, "8 Bit Per Pixel");
            }, cancellationToken);

        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            throw;
        }
        return bitPerPixelList;
    }
}
