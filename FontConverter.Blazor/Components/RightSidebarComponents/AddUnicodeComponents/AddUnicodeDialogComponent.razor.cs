using FontConverter.Blazor.Services;
using FontConverter.Blazor.ViewModels;
using FontConverter.SharedLibrary.Models;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;

namespace FontConverter.Blazor.Components.RightSidebarComponents.AddUnicodeComponents;

public partial class AddUnicodeDialogComponent
{
    [Inject]
    private DialogService _DialogService { get; set; } = default!;

    [Inject]
    public PredefinedDataService PredefinedData { get; set; } = default!;

    [Inject]
    public MainViewModel MainViewModel { get; set; } = default!;

    private IList<UnicodeBlock>? _SelectedBlocksList;
    private IList<UnicodeCharacter>? _SelectedCharachtersList;
    private UnicodeBlock _SelectedBlock = new();

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _SelectedBlocksList = new List<UnicodeBlock>() { PredefinedData.UnicodeBlockCollection.Blocks.Values.FirstOrDefault() ?? new() };
        _SelectedBlock = _SelectedBlocksList.FirstOrDefault() ?? new();
        _SelectedCharachtersList = null;
    }

    private void OnBlockChanged(IList<UnicodeBlock>? newValue)
    {
        if (newValue != null && newValue.Count > 0)
        {
            _SelectedBlocksList = newValue;
            _SelectedBlock = _SelectedBlocksList.FirstOrDefault() ?? new();
            _SelectedCharachtersList = null;
        }
    }

    private void OnCharacterChanged(IList<UnicodeCharacter>? newValue)
    {
        if (newValue != null && newValue.Count > 0)
        {
            foreach (var item in newValue)
            {
                if (item.GlyphID != null || item.GlyphID >= 0)
                    return;
            }
        }
        _SelectedCharachtersList = newValue;
    }

}
