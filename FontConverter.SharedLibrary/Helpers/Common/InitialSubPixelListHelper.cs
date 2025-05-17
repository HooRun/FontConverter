using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FontConverter.SharedLibrary.Helpers;

public static class InitialSubPixelListHelper
{
    public static async Task<SortedList<LVGLFontEnums.SUB_Pixel_ENUM, string>> InitialSubPixellList(CancellationToken cancellationToken = default)
    {
        SortedList<LVGLFontEnums.SUB_Pixel_ENUM, string> subPixelList = new();
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                subPixelList.Add(LVGLFontEnums.SUB_Pixel_ENUM.SUB_PIXEL_NONE, "None");
                subPixelList.Add(LVGLFontEnums.SUB_Pixel_ENUM.SUB_PIXEL_Horizontal, "Horizontal");
                subPixelList.Add(LVGLFontEnums.SUB_Pixel_ENUM.SUB_PIXEL_Vertical, "Vertical");
                subPixelList.Add(LVGLFontEnums.SUB_Pixel_ENUM.SUB_PIXEL_Both, "Both");
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
        return subPixelList;
    }

    
}
