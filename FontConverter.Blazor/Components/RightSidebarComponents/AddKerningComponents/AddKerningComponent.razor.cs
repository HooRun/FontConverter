using FontConverter.Blazor.Models.GlyphsView;
using FontConverter.Blazor.ViewModels;
using FontConverter.SharedLibrary.Models;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace FontConverter.Blazor.Components.RightSidebarComponents.AddKerningComponents;

public partial class AddKerningComponent : ComponentBase
{
    [Inject]
    private DialogService _DialogService { get; set; } = default!;

    [Inject]
    public MainViewModel MainViewModel { get; set; } = default!;

    [Parameter]
    public bool IsLeftKerning { get; set; } = false;

    Variant variant = Variant.Outlined;
    bool floatFieldLabel = true;

    private IList<GlyphItemModel>? _SelectedGlyphsList;
    private List<KernPair> _SelectedKernsPair = [];
    private int _KerningValue = 0;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _SelectedGlyphsList = null;
    }

    private void OnGlyphsChanged(IList<GlyphItemModel>? newValue)
    {
        if (newValue != null && newValue.Count > 0 && MainViewModel.LastSelectedGlyph!=null)
        {
            foreach (var item in newValue)
            {
                if (IsLeftKerning)
                {
                    if (MainViewModel.LastSelectedGlyph.LeftKernings.Any(x => x.Right == item.Index))
                        return;
                }
                else
                {
                    if (MainViewModel.LastSelectedGlyph.RightKernings.Any(x => x.Left == item.Index))
                        return;
                }
            }
        }
        _SelectedGlyphsList = newValue;
    }

    private void OnAddClick()
    {
        _SelectedKernsPair.Clear();
        if (_SelectedGlyphsList != null && _SelectedGlyphsList.Count > 0 && MainViewModel.LastSelectedGlyph != null)
        {
            foreach (var glyph in _SelectedGlyphsList)
            {
                if (IsLeftKerning)
                {
                    _SelectedKernsPair.Add(new KernPair()
                    {
                        Left = (ushort)MainViewModel.LastSelectedGlyph.Index,
                        Right = (ushort)glyph.Index,
                        Value = (short)_KerningValue
                    });
                }
                else
                {
                    _SelectedKernsPair.Add(new KernPair()
                    {
                        Left = (ushort)glyph.Index,
                        Right = (ushort)MainViewModel.LastSelectedGlyph.Index,
                        Value = (short)_KerningValue
                    });
                }
            }
        }
        _DialogService.Close(_SelectedKernsPair);
    }
}
