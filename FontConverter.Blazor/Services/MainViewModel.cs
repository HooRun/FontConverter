using FontConverter.SharedLibrary;

namespace FontConverter.Blazor.Services;

public class MainViewModel
{
    private readonly OpenTypeFontAnalyzer _FontAnalyzer;

    public MainViewModel(OpenTypeFontAnalyzer fontAnalyzer)
    {
        _FontAnalyzer = fontAnalyzer;
    }
}
