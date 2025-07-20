using static FontConverter.SharedLibrary.Helpers.FontTablesEnumHelper;

namespace FontConverter.SharedLibrary.Models;

public class GlyphSubstitution
{
    public GlyphSubstitutionType Type { get; set; }
    public ushort[] FromGlyphIds { get; set; } = Array.Empty<ushort>();
    public ushort[] ToGlyphIds { get; set; } = Array.Empty<ushort>();
    public string? FeatureTag { get; set; }
}

public class GlyphSubstitutionTable
{
    public ushort MajorVersion { get; set; }
    public ushort MinorVersion { get; set; }
    public List<GlyphSubstitution> Substitutions { get; set; } = new();
}