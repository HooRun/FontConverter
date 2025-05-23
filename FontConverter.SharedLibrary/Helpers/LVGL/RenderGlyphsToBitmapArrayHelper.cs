using FontConverter.SharedLibrary.Models;
using SkiaSharp;
using System.Runtime.InteropServices;
using static FontConverter.SharedLibrary.Helpers.LVGLFontEnums;

namespace FontConverter.SharedLibrary.Helpers;

public class RenderGlyphsToBitmapArrayHelper
{

    public static async Task<SortedList<int, LVGLGlyphBitmapData>> RenderGlyphsToBitmapArrayAsync(
        SKFont font,
        FontGlyfTable glyfTable,
        LVGLFontAdjusments lvFontAdjusment,
        int fontHeight,
        BIT_PER_PIXEL_ENUM bpp,
        IProgress<(int glyphIndex, double percentage)>? progress = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(glyfTable?.Glyphs);

        int threshold = lvFontAdjusment.Threshold;
        int totalGlyphs = glyfTable.Glyphs.Count;
        var glyphs = new SortedList<int, LVGLGlyphBitmapData>();
        int ChunkSize = Math.Max(1, totalGlyphs / 1000);
        int processedGlyphs = 0;

        using SKPaint paint = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            IsDither = false,
            Color = SKColors.Black
        };

        for (int i = 0; i < totalGlyphs; i += ChunkSize)
        {
            cancellationToken.ThrowIfCancellationRequested();
            int batchEnd = Math.Min(i + ChunkSize, totalGlyphs);

            for (int j = i; j < batchEnd; j++)
            {
                glyphs.Add(j, RenderGlyphToBitmapArray(font, (ushort)j, fontHeight, bpp, threshold, paint));
                processedGlyphs++;
            }
            progress?.Report((processedGlyphs, (double)processedGlyphs / totalGlyphs * 100));
            await Task.Delay(1).ConfigureAwait(false);
        }

        progress?.Report((totalGlyphs, 100.0));
        return glyphs;
    }

    public static LVGLGlyphBitmapData RenderGlyphToBitmapArray(SKFont font, ushort glyphIndex, int pixelHeight, BIT_PER_PIXEL_ENUM bpp, int threshold, SKPaint paint)
    {
        if (bpp is not (BIT_PER_PIXEL_ENUM.BPP_1 or BIT_PER_PIXEL_ENUM.BPP_2 or BIT_PER_PIXEL_ENUM.BPP_4 or BIT_PER_PIXEL_ENUM.BPP_8))
            return new LVGLGlyphBitmapData(glyphIndex, Array.Empty<byte>(), SKRectI.Empty);

        using var path = font.GetGlyphPath(glyphIndex);
        if (path == null || path.IsEmpty)
            return new LVGLGlyphBitmapData(glyphIndex, Array.Empty<byte>(), SKRectI.Empty);

        SKRectI bounds = SKRectI.Ceiling(path.TightBounds);
        int width = Math.Max(1, bounds.Width);
        int height = Math.Max(1, bounds.Height);
        int dataSize = width * height;

        var imageInfo = new SKImageInfo(width, height, SKColorType.Alpha8, SKAlphaType.Premul);
        IntPtr alphaDataPtr = Marshal.AllocHGlobal(dataSize);

        try
        {
            using var surface = SKSurface.Create(imageInfo, alphaDataPtr, width);
            if (surface == null)
                return new LVGLGlyphBitmapData(glyphIndex, Array.Empty<byte>(), bounds);

            var canvas = surface.Canvas;
            canvas.Clear();
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
        if (bpp == BIT_PER_PIXEL_ENUM.BPP_8)
            return alphaData;

        int pixelThreshold = threshold * 255 / 100;
        byte bitperpixel = (byte)bpp;
        int pixelsPerByte = 8 / (byte)bpp;
        int totalPixels = width * height;
        int byteCount = (totalPixels + pixelsPerByte - 1) / pixelsPerByte;
        var output = new byte[byteCount];

        for (int i = 0, outIndex = 0, bitIndex = 0, currentByte = 0; i < totalPixels; i++)
        {
            int value = (alphaData[i] >= pixelThreshold ? 255 : alphaData[i]) * ((1 << bitperpixel) - 1) / 255;
            currentByte = (currentByte << bitperpixel) | value;
            bitIndex += bitperpixel;

            if (bitIndex >= 8)
            {
                output[outIndex++] = (byte)currentByte;
                bitIndex = 0;
                currentByte = 0;
            }
            else if (i == totalPixels - 1 && bitIndex > 0)
            {
                output[outIndex] = (byte)(currentByte << (8 - bitIndex));
            }
        }

        return output;
    }
}