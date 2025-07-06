using FontConverter.Blazor.Components.LeftSidebarComponents;
using FontConverter.Blazor.Components.RightSidebarComponents.AddKerningComponents;
using FontConverter.Blazor.Components.RightSidebarComponents.AddUnicodeComponents;
using FontConverter.Blazor.Interfaces;
using FontConverter.Blazor.ViewModels;
using FontConverter.SharedLibrary.Models;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;

namespace FontConverter.Blazor.Components.RightSidebarComponents;

public partial class GlyphKerningsComponent : ComponentBase, IRerenderable
{
    [Inject]
    private DialogService _DialogService { get; set; } = default!;

    [Inject]
    private MainViewModel MainViewModel { get; set; } = default!;

    Variant variant = Variant.Outlined;
    bool floatFieldLabel = true;

    private KernPair? _LeftSelectedKern;
    private KernPair? _RightSelectedKern;

    private RadzenListBox<KernPair>? _LeftKerningList;
    private RadzenListBox<KernPair>? _RightKerningList;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        MainViewModel.RegisterComponent(nameof(GlyphKerningsComponent), this);
    }

    public async Task ForceRender()
    {
        _LeftKerningList?.Reset();
        _RightKerningList?.Reset();
        await InvokeAsync(StateHasChanged);
    }

    private async Task OnLeftKerningAddClick()
    {
        try
        {
            var dialogResult = await _DialogService.OpenAsync<AddKerningComponent>(
                    string.Empty,
                    new Dictionary<string, object>
                    {
                        { "IsLeftKerning", true },
                    },
                    new DialogOptions
                    {
                        ShowClose = false,
                        ShowTitle = false,
                    });
            var kernPairList = dialogResult as List<KernPair>;
            if (kernPairList !=null && kernPairList.Count>0 && MainViewModel.LastSelectedGlyph != null)
            {
                foreach (var kernPair in kernPairList)
                {
                    if (MainViewModel.LastSelectedGlyph.LeftKernings.Any(x => x.Right == kernPair.Right))
                        continue;
                    MainViewModel.LastSelectedGlyph.LeftKernings.Add(kernPair);
                }
            }
            MainViewModel.RerenderMany(nameof(RightSidebarComponent), nameof(FontContentsComponent));
        }
        catch (Exception)
        {

        }
    }

    private async Task OnRightKerningAddClick()
    {
        try
        {
            var dialogResult = await _DialogService.OpenAsync<AddKerningComponent>(
                    string.Empty,
                    new Dictionary<string, object>
                    {
                        { "IsLeftKerning", false },
                    },
                    new DialogOptions
                    {
                        ShowClose = false,
                        ShowTitle = false,
                    });
            var kernPairList = dialogResult as List<KernPair>;
            if (kernPairList != null && kernPairList.Count > 0 && MainViewModel.LastSelectedGlyph != null)
            {
                foreach (var kernPair in kernPairList)
                {
                    if (MainViewModel.LastSelectedGlyph.RightKernings.Any(x => x.Left == kernPair.Left))
                        continue;
                    MainViewModel.LastSelectedGlyph.RightKernings.Add(kernPair);
                }
            }
            MainViewModel.RerenderMany(nameof(RightSidebarComponent), nameof(FontContentsComponent));
        }
        catch (Exception)
        {

        }
    }

    private void OnLeftKerningRemoveClick()
    {
        if (MainViewModel.LastSelectedGlyph!=null && _LeftSelectedKern!=null)
        {
            if (MainViewModel.LastSelectedGlyph.LeftKernings.Contains(_LeftSelectedKern))
            {
                MainViewModel.LastSelectedGlyph.LeftKernings.Remove(_LeftSelectedKern);
            }
            MainViewModel.RerenderMany(nameof(RightSidebarComponent), nameof(FontContentsComponent));
        }
    }

    private void OnRightKerningRemoveClick()
    {
        if (MainViewModel.LastSelectedGlyph != null && _RightSelectedKern != null)
        {
            if (MainViewModel.LastSelectedGlyph.RightKernings.Contains(_RightSelectedKern))
            {
                MainViewModel.LastSelectedGlyph.RightKernings.Remove(_RightSelectedKern);
            }
            MainViewModel.RerenderMany(nameof(RightSidebarComponent), nameof(FontContentsComponent));
        }
    }
}
