using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LVGLFontConverter.Library.Data;

public record UnicodeBlock : IComparable<UnicodeBlock>
{
    public int Start { get; set; }
    public int End { get; set; }
    public string Name { get; set; }

    public UnicodeBlock(int start, int end, string name)
    {
        Start = start;
        End = end;
        Name = name ?? string.Empty;
    }

    public int CompareTo([AllowNull] UnicodeBlock other=null)
    {
        if (ReferenceEquals(other, null))
            return 1;

        int startComparison = Start.CompareTo(other.Start);
        if (startComparison != 0)
            return startComparison;

        return End.CompareTo(other.End);
    }
}

public record UnicodeCharacterName(int CodePoint, string Name);

public record StandardMacintoshGlyphName(int GlyphID, string Name);

public record GlyphToBitmapResult
{
    public int Index { get; set; }
    public byte[] Bitmap { get; set; }
    public SKRectI Bounds { get; set; }

    public GlyphToBitmapResult(int glyphIndex, byte[] bitmap, SKRectI bounds)
    {
        Index = glyphIndex;
        Bitmap = bitmap;
        Bounds = bounds;
    }
}