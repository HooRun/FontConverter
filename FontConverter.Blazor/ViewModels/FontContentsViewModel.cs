using FontConverter.SharedLibrary.Models;

namespace FontConverter.Blazor.ViewModels;

public class FontContentsViewModel : FontContentViewModel
{
    public FontContentsViewModel()
    {

    }

    public void CleanData()
    {
        Contents.Clear();
    }
}