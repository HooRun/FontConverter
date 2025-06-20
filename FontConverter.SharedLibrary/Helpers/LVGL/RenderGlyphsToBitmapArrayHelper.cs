﻿using FontConverter.SharedLibrary.Models;
using SkiaSharp;
using System.Runtime.InteropServices;
using static FontConverter.SharedLibrary.Helpers.LVGLFontEnums;

namespace FontConverter.SharedLibrary.Helpers;

public class RenderGlyphsToBitmapArrayHelper
{

    public static async Task<SortedList<int, LVGLGlyphBitmapData>> RenderGlyphsToBitmapArrayAsync(
        OpenTypeFont openTypeFont,
        LVGLFont lVGLFont,
        IProgress<(int glyphIndex, double percentage)>? progress = null,
        CancellationToken cancellationToken = default)
    {
        int threshold = lVGLFont.FontAdjusments.Threshold;
        int totalGlyphs = openTypeFont.GlyfTable.Glyphs.Count;
        var glyphs = new SortedList<int, LVGLGlyphBitmapData>();
        int chunkSize = Math.Max(1, totalGlyphs / 1000);
        if (chunkSize > 100) chunkSize = 100;
        int processedGlyphs = 0;

        int gammaValue = Math.Clamp(lVGLFont.FontAdjusments.Gamma, 0, 100);
        float gamma;
        if (gammaValue <= 50)
        {
            gamma = gammaValue / 50.0f; 
        }
        else
        {
            gamma = 1.0f + ((gammaValue - 50) * 9.0f / 50.0f);
        }

        openTypeFont.SKFont!.Edging = SKFontEdging.Alias;
        openTypeFont.SKFont!.Hinting = SKFontHinting.Full;
        openTypeFont.SKFont!.BaselineSnap = true;
        openTypeFont.SKFont!.Subpixel = false;

        using SKPaint paint = new()
        {
            IsAntialias = lVGLFont.FontAdjusments.AntiAlias,
            IsDither = lVGLFont.FontAdjusments.Dither,
            ColorFilter = lVGLFont.FontAdjusments.ColorFilter ? SKColorFilter.CreateBlendMode(SKColors.Black, SKBlendMode.SrcIn) : null,
            Shader = lVGLFont.FontAdjusments.Shader ? SKShader.CreateColor(SKColors.Black) : null,
            Style = (SKPaintStyle)lVGLFont.FontAdjusments.Style,
            Color = SKColors.Black,
            MaskFilter = SKMaskFilter.CreateGamma(gamma),
            StrokeWidth=5,
            
        };

        for (int i = 0; i < totalGlyphs; i += chunkSize)
        {
            cancellationToken.ThrowIfCancellationRequested();
            int batchEnd = Math.Min(i + chunkSize, totalGlyphs);

            for (int j = i; j < batchEnd; j++)
            {
                glyphs.Add(j, RenderGlyphToBitmapArray(openTypeFont.SKFont!, paint, (ushort)j, lVGLFont.FontSettings.FontSize, lVGLFont.FontSettings.FontBitPerPixel, threshold));
                processedGlyphs++;
            }
            progress?.Report((processedGlyphs, (double)processedGlyphs / totalGlyphs * 100));
            var delay = Math.Max(1, chunkSize / 50);
            await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
        }

        progress?.Report((totalGlyphs, 100.0));
        return glyphs;
    }

    public static LVGLGlyphBitmapData RenderGlyphToBitmapArray(SKFont font, SKPaint paint, ushort glyphIndex, int pixelHeight, BIT_PER_PIXEL_ENUM bpp, int threshold)
    {
        if (bpp is not (BIT_PER_PIXEL_ENUM.BPP_1 or BIT_PER_PIXEL_ENUM.BPP_2 or BIT_PER_PIXEL_ENUM.BPP_4 or BIT_PER_PIXEL_ENUM.BPP_8))
            return new LVGLGlyphBitmapData(glyphIndex, Array.Empty<byte>(), SKRectI.Empty);

        using var path = font.GetGlyphPath(glyphIndex);
        if (path == null || path.IsEmpty)
            return new LVGLGlyphBitmapData(glyphIndex, Array.Empty<byte>(), SKRectI.Empty);

        SKRectI bounds = SKRectI.Ceiling(path.TightBounds, true);
        int width = Math.Max(1, bounds.Width);
        int height = Math.Max(1, bounds.Height);
        int dataSize = width * height;

        var imageInfo = new SKImageInfo(width, height, SKColorType.Alpha8, SKAlphaType.Opaque);
        IntPtr alphaDataPtr = Marshal.AllocHGlobal(dataSize);

        try
        {
            using var surface = SKSurface.Create(imageInfo, alphaDataPtr, width);
            if (surface == null)
                return new LVGLGlyphBitmapData(glyphIndex, Array.Empty<byte>(), bounds);

            var canvas = surface.Canvas;
            canvas.Clear(SKColors.Transparent);
            canvas.SetMatrix(SKMatrix.CreateTranslation(-bounds.Left, -bounds.Top));
            canvas.DrawPath(path, paint);
            canvas.Flush();

            var alphaData = new byte[dataSize];
            Marshal.Copy(alphaDataPtr, alphaData, 0, dataSize);

            return new LVGLGlyphBitmapData(glyphIndex, ConvertAlphaToBpp(alphaData, width, height, bpp, threshold), bounds);
        }
        finally
        {
            Marshal.FreeHGlobal(alphaDataPtr);
        }
    }

    private static byte[] ConvertAlphaToBpp(byte[] alphaData, int width, int height, BIT_PER_PIXEL_ENUM bpp, int threshold)
    {
        int bppValue = (int)bpp;
        int maxValue = (1 << bppValue) - 1;

        int stride = (width * bppValue + 7) / 8;
        var output = new byte[stride * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int i = y * width + x;
                int alpha = alphaData[i];

                // Threshold cut
                if (alpha < threshold)
                    alpha = 0;

                // Normalize alpha to BPP value range
                int level = (int)Math.Round(alpha / 255.0 * maxValue);
                level = Math.Clamp(level, 0, maxValue);

                // Calculate bit position
                int bitIndex = x * bppValue;
                int byteIndex = y * stride + (bitIndex / 8);
                int bitOffset = 8 - bppValue - (bitIndex % 8); // MSB-first

                if (byteIndex < 0 || byteIndex >= output.Length || bitOffset < 0)
                    continue;

                output[byteIndex] |= (byte)(level << bitOffset);
            }
        }

        return output;
    }


}