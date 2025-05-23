using AutoMapper;
using FontConverter.Blazor.ViewModels;
using FontConverter.SharedLibrary;
using FontConverter.SharedLibrary.Models;

public class MainViewModel
{
    private readonly IMapper _mapper;

    public MainViewModel(
        IMapper mapper,
        OpenTypeFont openTypeFont,
        LVGLFont lvglFont)
    {
        _mapper = mapper;
        OpenTypeFont = openTypeFont;
        LVGLFont = lvglFont;
        FontSettingsViewModel = new();
        FontAdjusmentsViewModel = new();
        FontContentsViewModel = new();
        FontInformationsViewModel = new();

        MappingsFromModelToViewModel();
    }

    public OpenTypeFont OpenTypeFont { get; set; }
    public LVGLFont LVGLFont { get; set; }
    public FontSettingsViewModel FontSettingsViewModel { get; set; }
    public FontAdjusmentsViewModel FontAdjusmentsViewModel { get; set; }
    public FontContentsViewModel FontContentsViewModel { get; set; }
    public FontInformationsViewModel FontInformationsViewModel { get; set; }

    public void MappingsFromModelToViewModel()
    {
        FontSettingsViewModel = _mapper.Map<FontSettingsViewModel>(LVGLFont.FontSettings);
        FontAdjusmentsViewModel = _mapper.Map<FontAdjusmentsViewModel>(LVGLFont.FontAdjusments);
        FontContentsViewModel = _mapper.Map<FontContentsViewModel>(LVGLFont.FontContents);
        FontInformationsViewModel = _mapper.Map<FontInformationsViewModel>(LVGLFont.FontInformations);
    }

    public void MappingsFromViewModelToModel()
    {
        LVGLFont.FontSettings = _mapper.Map<LVGLFontSettings>(FontSettingsViewModel);
        LVGLFont.FontAdjusments = _mapper.Map<LVGLFontAdjusments>(FontAdjusmentsViewModel);
        LVGLFont.FontContents = _mapper.Map<LVGLFontContents>(FontContentsViewModel);
        LVGLFont.FontInformations = _mapper.Map<LVGLFontInformations>(FontInformationsViewModel);
    }

    public async Task SelectTreeItemAsync(FontContentViewModel selectedItem)
    {
        if (selectedItem == null || FontContentsViewModel?.Contents == null)
            return;

        await Task.Run(() => UpdateTreeSelection(FontContentsViewModel.Contents.Values, selectedItem));
    }

    private void UpdateTreeSelection(IEnumerable<FontContentViewModel> nodes, FontContentViewModel selectedItem)
    {
        if (nodes == null)
            return;

        foreach (var node in nodes)
        {
            node.IsSelected = node == selectedItem;
            UpdateTreeSelection(node.Children, selectedItem);
        }
    }
}