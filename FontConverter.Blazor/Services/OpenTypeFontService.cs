using FontConverter.SharedLibrary;

namespace FontConverter.Blazor.Services;

public class OpenTypeFontService
{
    private readonly OpenTypeFontAnalyzer _FontAnalyzer;

    public OpenTypeFontService(OpenTypeFontAnalyzer fontAnalyzer)
    {
        _FontAnalyzer = fontAnalyzer;
    }
}
