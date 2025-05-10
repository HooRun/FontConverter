using LVGLFontConverter.Library.Data;
using LVGLFontConverter.Library.Models;
using Microsoft.UI.Xaml.Documents;
using SkiaSharp;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using static LVGLFontConverter.Library.Helpers.GlyphToBitmapArray;

namespace LVGLFontConverter.Library.Helpers;

public static class GetGlyphInformation
{
    public static async Task<List<GlyphToBitmapResult>>RenderGlyphsAsync(
        SKFont font,
        FontGlyfTable glyfTable,
        LVGLFont lvFont,
        IProgress<(int glyphIndex, double percentage)> progress = null,
        CancellationToken cancellationToken = default)
    {
        if (glyfTable?.Glyphs == null)
            throw new ArgumentNullException("Invalid font table data.");

        int pixelHeight = lvFont.FontProperties.FontSize;
        byte bpp = (byte)lvFont.FontProperties.FontBitPerPixel;
        int threshold = lvFont.FontAdjusment.Threshold;
        int gamma = lvFont.FontAdjusment.Gamma;

        var concurrentGlyphs = new ConcurrentBag<GlyphToBitmapResult>();
        int totalGlyphs = glyfTable.Glyphs.Count;
        int percentageBag = totalGlyphs * 1 / 100;
        int processedGlyphs = 0;
        object reportLock = new object();

        try
        {
            void ReportProgress(int glyphIndex)
            {
                lock (reportLock)
                {
                    if (progress != null)
                    {
                        double percentage = (double)processedGlyphs / totalGlyphs * 100;
                        progress.Report((processedGlyphs, percentage));
                    }
                }
            }

            SKPaint paint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                IsDither = true,
                ColorFilter = SKColorFilter.CreateBlendMode(SKColors.Black, SKBlendMode.SrcIn),
                Shader = SKShader.CreateColor(SKColors.Black),
                Color = SKColors.Black
                //MaskFilter = SKMaskFilter.CreateGamma(gamma), // Optional: add a blur effect
            };

            await Task.Run(async () =>
            {
                var tasks = new List<Task>();

                foreach (var (glyph, index) in glyfTable.Glyphs.Select((g, i) => (g, i)))
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        var glyphIndex = (ushort)index;
                        // Render glyph
                        GlyphToBitmapResult glyphToBitmapResult = RenderGlyphToBitmap(font, glyphIndex, pixelHeight, bpp, threshold, gamma, paint);
                        concurrentGlyphs.Add(glyphToBitmapResult);
                        int current = Interlocked.Increment(ref processedGlyphs);
                        if (current % percentageBag == 0)
                        {
                            ReportProgress(glyphIndex);
                            await Task.Delay(500, cancellationToken);
                        }

                    }, cancellationToken));
                }

                await Task.WhenAll(tasks);
                ReportProgress(totalGlyphs);
                await Task.Delay(500, cancellationToken);
            }, cancellationToken);

            var glyphs = concurrentGlyphs.ToList();
            if (!glyfTable.Glyphs.Select((g, i) => i).SequenceEqual(glyphs.Select(g => g.Index)))
            {
                glyphs.Sort((g1, g2) => g1.Index.CompareTo(g2.Index));
            }

            paint.Dispose();
            paint = null;

            return glyphs;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public static async Task<List<LVGLFontGlyph>> GetGlyphsAsync(
        FontGlyfTable glyfTable,
        FontCmapTable cmapTable,
        FontHmtxTable hmtxTable,
        FontPostTable postTable,
        List<GlyphToBitmapResult> glyphsBitmapList,
        List<StandardMacintoshGlyphName> standardMacintoshGlyphNames,
        List<UnicodeCharacterName> unicodeCharacterNames,
        double scale,
        LVGLFont lvFont,
        IProgress<(int glyphIndex, double percentage)> progress = null,
        CancellationToken cancellationToken = default)
    {
        if (glyfTable?.Glyphs == null || cmapTable?.UnicodeToGlyphMap == null || hmtxTable?.GlyphMetrics == null)
            throw new ArgumentNullException("Invalid font table data.");

        var unicodeNameLookup = unicodeCharacterNames.ToDictionary(x => x.CodePoint, x => x.Name);
        int pixelHeight = lvFont.FontProperties.FontSize;
        byte bpp = (byte)lvFont.FontProperties.FontBitPerPixel;
        int threshold = lvFont.FontAdjusment.Threshold;
        int gamma = lvFont.FontAdjusment.Gamma;

        var concurrentGlyphs = new ConcurrentBag<LVGLFontGlyph>();
        int totalGlyphs = glyfTable.Glyphs.Count;
        int percentageBag = totalGlyphs * 1 / 100;
        int processedGlyphs = 0;
        object reportLock = new object();

        try
        {
            void ReportProgress(int glyphIndex)
            {
                lock (reportLock)
                {
                    if (progress != null)
                    {
                        double percentage = (double)processedGlyphs / totalGlyphs * 100;
                        progress.Report((processedGlyphs, percentage));
                    }
                }
            }

            await Task.Run(async () =>
            {
                var tasks = new List<Task>();

                foreach (var (glyph, index) in glyfTable.Glyphs.Select((g, i) => (g, i)))
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        var glyphIndex = (ushort)index;
                        var lvglGlyph = new LVGLFontGlyph();

                        // Set glyph name
                        if (postTable.GlyphNameIndex?.Count > glyphIndex)
                        {
                            ushort glyphNameIndex = postTable.GlyphNameIndex[glyphIndex];
                            if (glyphNameIndex > 257 && postTable.PascalStrings?.Count > glyphNameIndex - 258)
                            {
                                lvglGlyph.Name = postTable.PascalStrings[glyphNameIndex - 258];
                            }
                            else if (glyphNameIndex < standardMacintoshGlyphNames.Count)
                            {
                                lvglGlyph.Name = standardMacintoshGlyphNames[glyphNameIndex].Name;
                            }
                        }

                        // Set glyph properties
                        lvglGlyph.Index = glyphIndex;
                        lvglGlyph.AdvanceWidth = (int)Math.Ceiling(scale * hmtxTable.GlyphMetrics[glyphIndex].AdvanceWidth);
                        lvglGlyph.Width = (int)Math.Ceiling(scale * (glyph.XMax - glyph.XMin));
                        lvglGlyph.Height = (int)Math.Ceiling(scale * (glyph.YMax - glyph.YMin));
                        lvglGlyph.BitsPerPixel = bpp;
                        lvglGlyph.Threshold = threshold;
                        lvglGlyph.Gamma = gamma;
                        lvglGlyph.MaxCharWidth = lvFont.FontData.MaxCharWidth;
                        lvglGlyph.YAxisPosition = lvFont.FontProperties.YAxisPosition;
                        lvglGlyph.LineHeight = lvFont.FontProperties.LineHeight;
                        lvglGlyph.BaseLine = lvFont.FontProperties.BaseLine;

                        
                        lvglGlyph.Bitmap = glyphsBitmapList[glyphIndex].Bitmap;
                        lvglGlyph.BitmapSize = glyphsBitmapList[glyphIndex].Bitmap.Length;
                        lvglGlyph.BitmapWidth = glyphsBitmapList[glyphIndex].Bounds.Width;
                        lvglGlyph.BitmapHeight = glyphsBitmapList[glyphIndex].Bounds.Height;
                        lvglGlyph.OffsetX = glyphsBitmapList[glyphIndex].Bounds.Left;
                        lvglGlyph.OffsetY = lvglGlyph.BaseLine - (int)glyphsBitmapList[glyphIndex].Bounds.Bottom;

                        lvglGlyph.IsEmpty = lvglGlyph.BitmapWidth == 0 || lvglGlyph.BitmapHeight == 0;

                        // Map Unicode code points
                        var matchingKeys = cmapTable.UnicodeToGlyphMap
                            .Where(kvp => kvp.Value == glyphIndex)
                            .Select(kvp => kvp.Key)
                            .ToList();

                        if (matchingKeys.Count == 0)
                        {
                            lvglGlyph.IsUnMapped = true;
                        }
                        else if (matchingKeys.Count == 1)
                        {
                            lvglGlyph.IsSingleMapped = true;
                        }
                        else
                        {
                            lvglGlyph.IsMultiMapped = true;
                        }

                        foreach (uint codePoint in matchingKeys)
                        {
                            string name = unicodeNameLookup.TryGetValue((int)codePoint, out var unicodeName) ? unicodeName : string.Empty;
                            lvglGlyph.Unicodes.Add((codePoint, name));
                        }

                        concurrentGlyphs.Add(lvglGlyph);

                        int current = Interlocked.Increment(ref processedGlyphs);
                        if (current % percentageBag == 0)
                        {
                            ReportProgress(glyphIndex);
                            await Task.Delay(500, cancellationToken);
                        }

                    }, cancellationToken));
                }

                await Task.WhenAll(tasks);
                ReportProgress(totalGlyphs);
                await Task.Delay(500, cancellationToken);
            }, cancellationToken);

            var glyphs = concurrentGlyphs.ToList();
            if (!glyfTable.Glyphs.Select((g, i) => i).SequenceEqual(glyphs.Select(g => g.Index)))
            {
                glyphs.Sort((g1, g2) => g1.Index.CompareTo(g2.Index));
            }

            return glyphs;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            throw;
        }
    }

}

