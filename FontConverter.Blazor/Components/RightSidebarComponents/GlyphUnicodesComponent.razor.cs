using FontConverter.Blazor.Interfaces;
using FontConverter.Blazor.ViewModels;
using Microsoft.AspNetCore.Components;
using Radzen;
using FontConverter.SharedLibrary.Models;
using FontConverter.Blazor.Components.LeftSidebarComponents.FontFileComponents;
using FontConverter.Blazor.Components.RightSidebarComponents.AddUnicodeComponents;
using FontConverter.Blazor.Models;
using FontConverter.Blazor.Components.LeftSidebarComponents;
using System;

namespace FontConverter.Blazor.Components.RightSidebarComponents;

public partial class GlyphUnicodesComponent : ComponentBase, IRerenderable
{
    [Inject]
    private MainViewModel MainViewModel { get; set; } = default!;

    [Inject]
    public DialogService dialogService { get; set; } = default!;

    Variant variant = Variant.Outlined;
    bool floatFieldLabel = true;

    UnicodeCharacter? _SelectedChar;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        MainViewModel.RegisterComponent(nameof(GlyphUnicodesComponent), this);
    }

    public async Task ForceRender()
    {
        await InvokeAsync(StateHasChanged);
    }

    private async Task OnAddClick()
    {
        try
        {
            var dialogResult = await dialogService.OpenAsync<AddUnicodeDialogComponent>(
                    string.Empty,
                    new Dictionary<string, object>(),
                    new DialogOptions
                    {
                        ShowClose = false,
                        ShowTitle = false,
                    });

            if (dialogResult is not null && dialogResult is AddUnicodesResult res )
            {
                if (MainViewModel.LastSelectedGlyph != null && res.SelectedBlocksList != null && res.SelectedBlocksList.Count > 0 && res.SelectedCharachtersList != null && res.SelectedCharachtersList.Count > 0)
                {
                    var fontContent = MainViewModel.LVGLFont.FontContents;
                    var fontContentVM = MainViewModel.FontContentsViewModel;
                    var glyph = MainViewModel.LastSelectedGlyph!;

                    foreach (var blockItem in res.SelectedBlocksList)
                    {
                        if (!glyph.Blocks.ContainsKey((blockItem.Start, blockItem.End)))
                        {
                            glyph.Blocks.Add((blockItem.Start, blockItem.End), blockItem);
                        }
                        fontContentVM.Contents[fontContent.UnicodesHeader].Count += res.SelectedCharachtersList.Count;
                        if (!fontContentVM.Contents[fontContent.UnicodesHeader].Contents.ContainsKey(blockItem.Start.ToString()))
                        {
                            string subTitle = $"Range: 0x{blockItem.Start:X04} - 0x{blockItem.End:X04}";
                            fontContentVM.Contents[fontContent.UnicodesHeader]
                                .Contents
                                .TryAdd(blockItem.Start.ToString(), new FontContentViewModel(blockItem.Name, subTitle, fontContent.UnicodeRangeIcon, 1, false, null, new SortedList<string, FontContentViewModel>()));
                            fontContentVM.Contents[fontContent.UnicodesHeader]
                                .Contents[blockItem.Start.ToString()]
                                .Items.Add(glyph.Index);
                        }
                        else
                        {
                            fontContentVM.Contents[fontContent.UnicodesHeader].Contents[blockItem.Start.ToString()].Count ++;
                        }
                        break;
                    }
                    foreach (var charItem in res.SelectedCharachtersList)
                    {
                        charItem.GlyphID = glyph.Index;
                        if (!glyph.CodePoints.ContainsKey(charItem.CodePoint))
                        {
                            glyph.CodePoints.Add(charItem.CodePoint, charItem);
                        }
                    }
                    if (glyph.CodePoints.Count > 0 && glyph.IsUnMapped)
                    {
                        if (fontContentVM.Contents[fontContent.GlyphsHeader]
                            .Contents[fontContent.UnMappedGlyphsHeader]
                            .Items.Contains(glyph.Index))
                        {
                            fontContentVM.Contents[fontContent.GlyphsHeader]
                            .Contents[fontContent.UnMappedGlyphsHeader]
                            .Items.Remove(glyph.Index);

                            fontContentVM.Contents[fontContent.GlyphsHeader]
                            .Contents[fontContent.UnMappedGlyphsHeader]
                            .Count--;
                        }
                        glyph.IsUnMapped = false;
                    }
                    if (glyph.CodePoints.Count == 1)
                    {
                        fontContentVM.Contents[fontContent.GlyphsHeader]
                            .Contents[fontContent.UnMappedGlyphsHeader]
                            .Items.Add(glyph.Index);

                        fontContentVM.Contents[fontContent.GlyphsHeader]
                        .Contents[fontContent.UnMappedGlyphsHeader]
                        .Count++;
                        glyph.IsSingleMapped = true;
                    }
                    else if (glyph.CodePoints.Count > 1)
                    {
                        if (fontContentVM.Contents[fontContent.GlyphsHeader]
                            .Contents[fontContent.SingleMappedGlyphsHeader]
                            .Items.Contains(glyph.Index))
                        {
                            fontContentVM.Contents[fontContent.GlyphsHeader]
                            .Contents[fontContent.SingleMappedGlyphsHeader]
                            .Items.Remove(glyph.Index);

                            fontContentVM.Contents[fontContent.GlyphsHeader]
                            .Contents[fontContent.SingleMappedGlyphsHeader]
                            .Count--;
                        }
                        if (!glyph.IsMultiMapped)
                        {
                            fontContentVM.Contents[fontContent.GlyphsHeader]
                            .Contents[fontContent.MultiMappedGlyphsHeader]
                            .Items.Add(glyph.Index);

                            fontContentVM.Contents[fontContent.GlyphsHeader]
                            .Contents[fontContent.MultiMappedGlyphsHeader]
                            .Count++;
                        }
                        glyph.IsSingleMapped = false;
                        glyph.IsMultiMapped = true;
                    }

                    MainViewModel.RerenderMany(nameof(RightSidebarComponent), nameof(FontContentsComponent));
                }
            }
        }
        catch (Exception)
        {

        }

    }

    private void OnRemoveClick()
    {
        if (MainViewModel.LastSelectedGlyph == null || _SelectedChar == null || _SelectedChar.ParentBlock == null) 
            return;

        var fontContent = MainViewModel.LVGLFont.FontContents;
        var fontContentVM = MainViewModel.FontContentsViewModel;
        var glyph = MainViewModel.LastSelectedGlyph;
        string blockStart = _SelectedChar.ParentBlock.Start.ToString();

        fontContentVM.Contents[fontContent.UnicodesHeader].Count--;
        if (fontContentVM.Contents[fontContent.UnicodesHeader].Contents.ContainsKey(blockStart))
        {
            fontContentVM.Contents[fontContent.UnicodesHeader]
                .Contents[blockStart]
                .Items.Remove(glyph.Index);
            fontContentVM.Contents[fontContent.UnicodesHeader]
                .Contents[blockStart]
                .Count--;
            if (fontContentVM.Contents[fontContent.UnicodesHeader]
                .Contents[blockStart]
                .Items.Count == 0)
            {
                fontContentVM.Contents[fontContent.UnicodesHeader]
                .Contents.Remove(blockStart);
                if (!glyph.Blocks.ContainsKey((_SelectedChar.ParentBlock.Start, _SelectedChar.ParentBlock.End)))
                {
                    glyph.Blocks.Remove((_SelectedChar.ParentBlock.Start, _SelectedChar.ParentBlock.End));
                }
            }
        }


        _SelectedChar.GlyphID = null;
        if (glyph.CodePoints.ContainsKey(_SelectedChar.CodePoint))
        {
            glyph.CodePoints.Remove(_SelectedChar.CodePoint);
        }

        glyph.IsUnMapped = false;
        glyph.IsSingleMapped = false;
        glyph.IsMultiMapped = false;

        if (glyph.CodePoints.Count == 0)
        {
            if (fontContentVM.Contents[fontContent.GlyphsHeader]
                .Contents[fontContent.MultiMappedGlyphsHeader]
                .Items.Contains(glyph.Index))
            {
                fontContentVM.Contents[fontContent.GlyphsHeader]
                .Contents[fontContent.MultiMappedGlyphsHeader]
                .Items.Remove(glyph.Index);

                fontContentVM.Contents[fontContent.GlyphsHeader]
                .Contents[fontContent.MultiMappedGlyphsHeader]
                .Count--;
            }
            if (fontContentVM.Contents[fontContent.GlyphsHeader]
                .Contents[fontContent.SingleMappedGlyphsHeader]
                .Items.Contains(glyph.Index))
            {
                fontContentVM.Contents[fontContent.GlyphsHeader]
                .Contents[fontContent.SingleMappedGlyphsHeader]
                .Items.Remove(glyph.Index);

                fontContentVM.Contents[fontContent.GlyphsHeader]
                .Contents[fontContent.SingleMappedGlyphsHeader]
                .Count--;
            }
            if (!fontContentVM.Contents[fontContent.GlyphsHeader]
                .Contents[fontContent.UnMappedGlyphsHeader]
                .Items.Contains(glyph.Index))
            {
                fontContentVM.Contents[fontContent.GlyphsHeader]
                .Contents[fontContent.UnMappedGlyphsHeader]
                .Items.Add(glyph.Index);

                fontContentVM.Contents[fontContent.GlyphsHeader]
                .Contents[fontContent.UnMappedGlyphsHeader]
                .Count++;
            }
            glyph.IsUnMapped = true;
        }
        else if (glyph.CodePoints.Count == 1)
        {
            if (fontContentVM.Contents[fontContent.GlyphsHeader]
                .Contents[fontContent.MultiMappedGlyphsHeader]
                .Items.Contains(glyph.Index))
            {
                fontContentVM.Contents[fontContent.GlyphsHeader]
                .Contents[fontContent.MultiMappedGlyphsHeader]
                .Items.Remove(glyph.Index);

                fontContentVM.Contents[fontContent.GlyphsHeader]
                .Contents[fontContent.MultiMappedGlyphsHeader]
                .Count--;
            }
            if (!fontContentVM.Contents[fontContent.GlyphsHeader]
                .Contents[fontContent.SingleMappedGlyphsHeader]
                .Items.Contains(glyph.Index))
            {
                fontContentVM.Contents[fontContent.GlyphsHeader]
                .Contents[fontContent.SingleMappedGlyphsHeader]
                .Items.Add(glyph.Index);

                fontContentVM.Contents[fontContent.GlyphsHeader]
                .Contents[fontContent.SingleMappedGlyphsHeader]
                .Count++;
            }

            glyph.IsSingleMapped = true;
        }
        else if (glyph.CodePoints.Count > 1)
        {
            if (fontContentVM.Contents[fontContent.GlyphsHeader]
                .Contents[fontContent.MultiMappedGlyphsHeader]
                .Items.Contains(glyph.Index))
            {
                fontContentVM.Contents[fontContent.GlyphsHeader]
                .Contents[fontContent.MultiMappedGlyphsHeader]
                .Items.Remove(glyph.Index);

                fontContentVM.Contents[fontContent.GlyphsHeader]
                .Contents[fontContent.MultiMappedGlyphsHeader]
                .Count--;
            }
            glyph.IsMultiMapped = true;
        }

        MainViewModel.RerenderMany(nameof(RightSidebarComponent), nameof(FontContentsComponent));
    }


}
