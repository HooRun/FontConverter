using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace FontConverter.SharedLibrary.Models;

public class LVGLGlyph
{
    public LVGLGlyph()
    {
        Index = -1;
        Name = string.Empty;
        Description = string.Empty;
        Bitmap = [];
        Descriptor = new();
        Adjusments = new();
        CodePoints = new();
        Blocks = new();

        LeftKernings = [];
        RightKernings = [];

        SVG = new();

        IsEmpty = false;
        IsUnMapped = false;
        IsSingleMapped = false;
        IsMultiMapped = false;

        GlyphGroupByContentHeader = string.Empty;
        GlyphGroupByUnicodeRangeHeader = string.Empty;
    }

    public LVGLGlyph(LVGLGlyph glyph) : this()
    {
        Index = glyph.Index;
        Name = glyph.Name;
        Description = glyph.Description;
        Bitmap = glyph.Bitmap;
        Descriptor = glyph.Descriptor;
        Adjusments = glyph.Adjusments;
        CodePoints = glyph.CodePoints;
        Blocks = glyph.Blocks;

        LeftKernings = glyph.LeftKernings;
        RightKernings = glyph.RightKernings;

        SVG = glyph.SVG;

        IsEmpty = glyph.IsEmpty;
        IsUnMapped = glyph.IsUnMapped;
        IsSingleMapped = glyph.IsSingleMapped;
        IsMultiMapped = glyph.IsMultiMapped;

        GlyphGroupByContentHeader = glyph.GlyphGroupByContentHeader;
        GlyphGroupByUnicodeRangeHeader = glyph.GlyphGroupByUnicodeRangeHeader;
    }

    public int Index { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public byte[] Bitmap { get; set; }
    public LVGLGlyphDescriptor Descriptor { get; set; }
    public LVGLFontAdjusments Adjusments { get; set; }
    public SortedDictionary<uint, UnicodeCharacter> CodePoints { get; set; }
    public SortedDictionary<uint, UnicodeBlock> Blocks { get; set; }

    public List<KernPair> LeftKernings { get; set; }
    public List<KernPair> RightKernings { get; set; }

    public LVGLGlyphSVG SVG { get; set; }

    public bool IsEmpty { get; set; }
    public bool IsUnMapped { get; set; }
    public bool IsSingleMapped { get; set; }
    public bool IsMultiMapped { get; set; }

    public int Offset { get; set; }

    public string UnicodesStrings => CodePoints.Count > 0 ? string.Join(", ", CodePoints.Values.Select(g => g.CodePointString)) : string.Empty;
    public IList<string> Unicodes => CodePoints.Count > 0 ? CodePoints.Values.Select(g => g.CodePointString).Append(" ").ToList() : new List<string>() { string.Empty };
    public string GlyphGroupByContentHeader { get; set; }
    public string GlyphGroupByUnicodeRangeHeader { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is not LVGLGlyph other) return false;
        return Index == other.Index;
    }

    public override int GetHashCode() => Index.GetHashCode();
}
