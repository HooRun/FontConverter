using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LVGLFontConverter.Library.Data;

public record UnicodeBlock(int Start, int End, string Name) : IComparable<UnicodeBlock>
{
    public int CompareTo(UnicodeBlock? other)
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