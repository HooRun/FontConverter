using FontConverter.SharedLibrary;
using FontConverter.SharedLibrary.Helpers;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace FontConverter.Blazor.Components.LeftSidebarComponents;

public partial class FontAdjusmentsComponent : ComponentBase
{
    [Inject]
    public FontConverterLib fontConverterLib { get; set; } = default!;

    int colLeft = 4;
    int colRight = 8;

    Variant variant = Variant.Outlined;
    bool floatFieldLabel = true;

    bool antiAliasValue = true;
    bool ditherValue = true;
    bool colorFilterValue = true;
    bool shaderValue = true;

    LVGLFontEnums.GLYPH_STYLE selectedGlyphStyle = LVGLFontEnums.GLYPH_STYLE.STYLE_FILL;
    
    int gammaValue = 50;
    int thresholdValue = 0;
}
