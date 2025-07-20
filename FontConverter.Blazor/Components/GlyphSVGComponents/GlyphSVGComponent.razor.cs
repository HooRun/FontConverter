using FontConverter.SharedLibrary.Models;
using Microsoft.AspNetCore.Components;

namespace FontConverter.Blazor.Components.GlyphSVGComponents;

public partial class GlyphSVGComponent
{
    [Parameter]
    public LVGLGlyphSVG SVG { get; set; } = new LVGLGlyphSVG();

    [Parameter]
    public int Height { get; set; } = 0;

    [Parameter]
    public int MaxWidth { get; set; } = 0;
}
