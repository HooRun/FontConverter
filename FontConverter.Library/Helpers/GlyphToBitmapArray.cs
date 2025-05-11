using LVGLFontConverter.Library.Data;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace LVGLFontConverter.Library.Helpers;

public static class GlyphToBitmapArray
{
    public static GlyphToBitmapResult RenderGlyphToBitmap(SKFont font, ushort glyphIndex, int pixelHeight, byte bpp, int threshold, int gamma, SKPaint paint)
    {
        GlyphToBitmapResult bResponce = new GlyphToBitmapResult(glyphIndex, Array.Empty<byte>(), new SKRectI());
        if (bpp != 1 && bpp != 2 && bpp != 4 && bpp != 8)
        {
            return bResponce;
        }

        //using var paint = new SKPaint
        //{
        //    IsAntialias = true,
        //    Style = SKPaintStyle.Fill,
        //    IsDither = true,
        //    ColorFilter = SKColorFilter.CreateBlendMode(SKColors.Black, SKBlendMode.SrcIn),
        //    Shader = SKShader.CreateColor(SKColors.Black),
        //    Color = SKColors.Black
        //    //MaskFilter = SKMaskFilter.CreateGamma(gamma), // Optional: add a blur effect
        //};

        using var path = font.GetGlyphPath((ushort)glyphIndex);
        if (path == null)
        {
            return bResponce;
        }

        SKRectI bounds = SKRectI.Ceiling(path.Bounds, true);
        int padding = 0;
        int width = bounds.Width + padding * 2;
        int height = bounds.Height + padding * 2;

        var imageInfo = new SKImageInfo(width, height, SKColorType.Alpha8, SKAlphaType.Premul);
        using var surface = SKSurface.Create(imageInfo);
        if (surface == null)
        {
            return bResponce;
        }

        surface.Canvas.Clear(SKColors.Transparent);
        var matrix = SKMatrix.CreateTranslation(-bounds.Left + padding, -bounds.Top + padding);
        surface.Canvas.SetMatrix(matrix);
        surface.Canvas.DrawPath(path, paint);
        surface.Canvas.Flush();

        using var image = surface.Snapshot();
        if (image == null)
        {
            return bResponce;
        }

        using var pixmap = image.PeekPixels();
        if (pixmap == null)
        {
            return bResponce;
        }

        int dataSize = width * height;
        IntPtr alphaDataPtr = Marshal.AllocHGlobal(dataSize);
        try
        {
            if (!pixmap.ReadPixels(imageInfo, alphaDataPtr, width))
            {
                return bResponce;
            }

            var alphaData = new byte[dataSize];
            Marshal.Copy(alphaDataPtr, alphaData, 0, dataSize);

            bResponce.Bitmap = ConvertAlphaToBpp(alphaData, width, height, bpp, threshold);
            bResponce.Bounds = bounds;
            return bResponce;
        }
        finally
        {
            Marshal.FreeHGlobal(alphaDataPtr);
        }
    }


    private static byte[] ConvertAlphaToBpp(byte[] alphaData, int width, int height, int bpp, int threshold)
    {
        int pixelThreshold = (threshold * 255 / 100);
        if (bpp == 8)
        {
            return alphaData; // No conversion needed for 8bpp
        }

        int pixelsPerByte = 8 / bpp;
        int totalPixels = width * height;
        int byteCount = (totalPixels + pixelsPerByte - 1) / pixelsPerByte;
        var output = new byte[byteCount];
        int bitIndex = 0;
        int outIndex = 0;
        byte currentByte = 0;

        for (int i = 0; i < totalPixels; i++)
        {
            int alpha = alphaData[i] >= pixelThreshold ? 255 : alphaData[i];
            int max = (1 << bpp) - 1;
            int value = (int)Math.Round((alpha / 255.0) * max);

            currentByte = (byte)(currentByte << bpp | value);
            bitIndex += bpp;

            if (bitIndex >= 8)
            {
                output[outIndex++] = currentByte;
                bitIndex = 0;
                currentByte = 0;
            }
        }

        if (bitIndex > 0)
        {
            output[outIndex] = (byte)(currentByte << (8 - bitIndex));
        }

        return output;
    }

}
