using FontConverter.SharedLibrary.Models;

namespace FontConverter.Blazor.Models;

public class AddUnicodesResult
{
    public AddUnicodesResult()
    {
        
    }

    public AddUnicodesResult(IList<UnicodeBlock>? selectedBlocksList, IList<UnicodeCharacter>? selectedCharachtersList) : this()
    {
        SelectedBlocksList = selectedBlocksList;
        SelectedCharachtersList = selectedCharachtersList;
    }

    public IList<UnicodeBlock>? SelectedBlocksList { get; set; }
    public IList<UnicodeCharacter>? SelectedCharachtersList { get; set; }
}
