using FontConverter.Blazor.Services;
using FontConverter.Blazor.ViewModels;
using FontConverter.SharedLibrary.Models;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using static FontConverter.SharedLibrary.Helpers.UCDEnumsHelper;

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
        _SelectedBlocksList = new List<UnicodeBlock>() { PredefinedData.Blocks.Values.FirstOrDefault() ?? new() };
        _SelectedBlock = _SelectedBlocksList.FirstOrDefault() ?? new();
        _SelectedCharachtersList = null;
    }

    private async Task OnBlockChangedAsync(IList<UnicodeBlock>? newValue)
    {
        if (newValue != null && newValue.Count > 0)
        {
            _SelectedBlocksList = newValue;
            _SelectedBlock = _SelectedBlocksList.FirstOrDefault() ?? new();
            _SelectedCharachtersList = null;

            await CompleteBlockAsync(_SelectedBlock);
            StateHasChanged();
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

    public static async Task CompleteBlockAsync(UnicodeBlock block)
    {
        if (block.Characters.Count >= block.Length || string.IsNullOrEmpty(block.Name))
            return;

        await Task.Yield();

        uint end = block.End;
        for (uint ch = block.Start; ch <= end; ch++)
        {
            if (!block.Characters.ContainsKey(ch))
            {
                block.Characters[ch] = new UnicodeCharacter
                {
                    CodePoint = ch,
                    Name = $"U+{ch:X6}",
                    DecompositionType = DecompositionTypeEnum.DECOMPOSITION_TYPE_NONE,
                    DecompositionMapping = [],
                    Block = block.Start
                };
            }

            if ((ch - block.Start) % 100 == 0)
                await Task.Delay(1); 
        }
    }

}
