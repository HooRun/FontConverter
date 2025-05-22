using FontConverter.SharedLibrary;
using FontConverter.SharedLibrary.Helpers;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace FontConverter.Blazor.Components.LeftSidebarComponents;

public partial class FontSettingsComponent : ComponentBase
{
    [Inject]
    public FontConverterLib fontConverterLib { get; set; } = default!;

    Variant variant = Variant.Outlined;
    bool floatFieldLabel = true;

    int fontSize;
    LVGLFontEnums.BIT_PER_PIXEL_ENUM selectedBPP = LVGLFontEnums.BIT_PER_PIXEL_ENUM.BPP_8;
    LVGLFontEnums.SUB_Pixel_ENUM selectedSubPixel = LVGLFontEnums.SUB_Pixel_ENUM.SUB_PIXEL_NONE;
    string selectedFallbackFont = string.Empty;


    

}
