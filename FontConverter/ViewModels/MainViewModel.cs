using LVGLFontConverter.Commands;
using LVGLFontConverter.Library;
using LVGLFontConverter.Library.Models;
using LVGLFontConverter.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.Storage;
using LVGLFontConverter.Helpers;
using LVGLFontConverter.Contracts.Services;
using LVGLFontConverter.Services;
using Microsoft.UI.Xaml.Navigation;
using LVGLFontConverter.Views;
using System.Text.RegularExpressions;
using LVGLFontConverter.UserControls;
using AutoMapper;
using System.Reflection;
using LVGLFontConverter.Library.Models.OpenType;
using static LVGLFontConverter.Library.Helpers.LVGLFontEnums;
using System.Diagnostics;
using Microsoft.UI.Xaml.Controls;
using LVGLFontConverter.Contracts.ViewModels;
using System.Windows.Input;


namespace LVGLFontConverter.ViewModels;

public partial class MainViewModel: BaseViewModel
{
    private readonly IMapper _mapper;

    public FontPropertiesViewModel FontPropertiesVM { get; set;  }
    public FontDataViewModel FontDataVM { get; set; }
    public FontAdjusmentViewModel FontAdjusmentVM { get; set; }
    public FontLoaderViewModel FontLoaderVM { get; set; }
    private FontLoader _FontLoaderPage;

    public MainViewModel(
        IMapper mapper, 
        LVGLFontConverter.Library.FontConverter fontConverter, 
        INavigationService navigationService, 
        FontPropertiesViewModel fontPropertiesVM, 
        FontDataViewModel fontDataVM,
        FontAdjusmentViewModel fontAdjusmentVM,
        FontLoaderViewModel fontLoaderVM,
        FontLoader fontLoaderPage)
    {
        _mapper = mapper;
        FontConverter = fontConverter;
        

        FontDataVM = fontDataVM;
        FontPropertiesVM = fontPropertiesVM;
        FontAdjusmentVM = fontAdjusmentVM;
        FontLoaderVM = fontLoaderVM;

        _FontLoaderPage = fontLoaderPage;

        _mapper.Map(FontConverter.LVGLFont.FontData, FontDataVM);
        _mapper.Map(FontConverter.LVGLFont.FontProperties, FontPropertiesVM);
        _mapper.Map(FontConverter.LVGLFont.FontAdjusment, FontAdjusmentVM);

        SpltviewPaneButtonCommand = new RelayCommand(SplitViewChangePaneStatus);
        FontFileOpenPickerCommand = new AsyncRelayCommand(FontFileOpenPicker, new Func<bool>(CanExecuteOpenFilePicker));
        SearchCommand = new AsyncRelayCommand<string>(SearchAsync);
        SuggestionChosenCommand = new RelayCommand<object>(OnSuggestionChosen);
        QuerySubmittedCommand = new RelayCommand<string>(OnQuerySubmitted);
        ConvertFont = new AsyncRelayCommand(ConvertFontToLVGL);

        NavigationService = navigationService;
        NavigationService.Navigated += OnNavigatd;
    }

    private async Task ConvertFontToLVGL()
    {
        _mapper.Map(FontPropertiesVM, FontConverter.LVGLFont.FontProperties);
        _mapper.Map(FontAdjusmentVM, FontConverter.LVGLFont.FontAdjusment);

        ContentDialog dialog = new ContentDialog();
        FontLoaderVM.FontNamePath = FontNamePath;
        FontLoaderVM.FontLoaderContentDialog = dialog;
        dialog.Content = _FontLoaderPage;
        dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
        dialog.CloseButtonText = "Cancel";
        dialog.PrimaryButtonText = "Apply";
        dialog.IsPrimaryButtonEnabled = false;
        dialog.XamlRoot = App.MainWindow.Content.XamlRoot;
        ContentDialogResult dialogResult =  await dialog.ShowAsync();

        dialog.Content = null;
        FontLoaderVM.FontLoadingCancellationToken?.CancelAfter(1);
        if (dialogResult==ContentDialogResult.None)
        {

        }
        else if (dialogResult == ContentDialogResult.Primary)
        {
            _mapper.Map(FontConverter.LVGLFont.FontData, FontDataVM);
            _mapper.Map(FontConverter.LVGLFont.FontProperties, FontPropertiesVM);
            _mapper.Map(FontConverter.LVGLFont.FontAdjusment, FontAdjusmentVM);
            LVFont = new();
            LVFont = FontConverter.LVGLFont;
            NavigationService?.Frame?.UpdateLayout();
            InitialContentTreeView();
        }

    }

    private void OnNavigatd(object sender, NavigationEventArgs e)
    {
        
    }

    private async Task FontFileOpenPicker()
    {
        IsFontPickFileButtonEnabled = false;
        FileOpenPicker openPicker = new FileOpenPicker();
        openPicker.ViewMode = PickerViewMode.List;
        openPicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
        openPicker.FileTypeFilter.Add(".ttf");
        openPicker.FileTypeFilter.Add(".otf");

        WinRT.Interop.InitializeWithWindow.Initialize(openPicker, App._HWND);

        StorageFile file = await openPicker.PickSingleFileAsync();

        if (file != null)
        {
            FontNamePath = file.Path;
        }

        IsFontPickFileButtonEnabled = true;
    }

    private void SplitViewChangePaneStatus()
    {
        if (SplitViewPaneOpen)
        {
            SplitViewPaneButtonIcon = Regex.Unescape("Icon_App_Pane_Close".GetLocalized());
            SplitViewPaneOpen = false;
        }
        else
        {
            SplitViewPaneButtonIcon = Regex.Unescape("Icon_App_Pane_Open".GetLocalized());
            SplitViewPaneOpen = true;
        }
    }

    private async Task SearchAsync(string? query)
    {
        await Task.Delay(10);
        if (string.IsNullOrEmpty(query))
        {
            SugestSystemFonts = FontConverter.SystemFonts.ToList();
        }
        else
        {
            SugestSystemFonts = FontConverter.SystemFonts
                .Where(item => item.ToLower().Contains(query.ToLower()))
                .OrderBy(item => item)
                .ToList();
        }
            
        
    }

    private void OnSuggestionChosen(object? item)
    {
        
    }

    private void OnQuerySubmitted(string? query)
    {
        
    }

    public INavigationService NavigationService { get; }

    //private const string _splitViewPaneOpenIcon = "\uEA49";
    //private const string _splitViewPaneCloseIcon = "\uEA5B";

    public RelayCommand SpltviewPaneButtonCommand { get; }
    public AsyncRelayCommand FontFileOpenPickerCommand;
    public AsyncRelayCommand<string> SearchCommand { get; }
    public RelayCommand<object> SuggestionChosenCommand { get; }
    public RelayCommand<string> QuerySubmittedCommand { get; }
    public AsyncRelayCommand ConvertFont { get; }


    public LVGLFontConverter.Library.FontConverter FontConverter { get; private set; }

    private List<string> _sugestSystemFonts = [];
    public List<string> SugestSystemFonts
    {
        get { return _sugestSystemFonts; }
        set { SetProperty(ref _sugestSystemFonts, value); }
    }

    private GridLength _titleBarHeight;
    public GridLength TitleBarHight
    {
        get { return _titleBarHeight; }
        set { SetProperty(ref _titleBarHeight, value); }
    }

    private LVGLFont _LVFont=new();
    public LVGLFont LVFont
    {
        get { return _LVFont; }
        set { SetProperty(ref _LVFont, value); }
    }

    private string _SplitViewPaneButtonIcon = Regex.Unescape("Icon_App_Pane_Close".GetLocalized());
    public string SplitViewPaneButtonIcon
    {
        get { return _SplitViewPaneButtonIcon; }
        set { SetProperty(ref _SplitViewPaneButtonIcon, value); }
    }

    private bool _SplitViewPaneOpen = true;
    public bool SplitViewPaneOpen
    {
        get { return _SplitViewPaneOpen; }
        set { SetProperty(ref _SplitViewPaneOpen, value); }
    }

    private ObservableCollection<FontContentTreeView> _ContentTreeView = new();
    public ObservableCollection<FontContentTreeView> ContentTreeView
    {
        get { return _ContentTreeView; }
        set { SetProperty(ref _ContentTreeView, value); }
    }

    private bool _isFontPickFileButtonEnabled = true;
    public bool IsFontPickFileButtonEnabled
    {
        get { return _isFontPickFileButtonEnabled; }
        set
        {
            SetProperty(ref _isFontPickFileButtonEnabled, value);
            FontFileOpenPickerCommand.RaiseCanExecuteChanged();
        }
    }

    private string _FontNamePath = string.Empty;
    public string FontNamePath
    {
        get { return _FontNamePath; }
        set { SetProperty(ref _FontNamePath, value); }
    }



    private bool CanExecuteOpenFilePicker()
    {
        return IsFontPickFileButtonEnabled;
    }

    private void InitialContentTreeView()
    {
        ContentTreeView.Clear();

        ContentTreeView.Insert(0, new FontContentTreeView()
        {
            Header = "Text_Item_FontContent_Glyphs".GetLocalized(),
            Icon = "\uF158",
            Count = LVFont.Glyphs.Count,
            IsSelected = true,
        });

        ContentTreeView.Insert(1, new FontContentTreeView() 
        { 
            Header = "Text_Item_FontContent_Unicodes".GetLocalized(), 
            Icon = "\uF2B7", 
            Count = LVFont.Unicodes.Count,
        });

        ContentTreeView[0].Insert(0, new FontContentTreeView() 
        { 
            Header = "Text_Item_FontContent_Glyphs_Empty".GetLocalized(), 
            Icon = "\uF157", 
            Count=(int)LVFont.EmptyGlyphsCount,
        });

        ContentTreeView[0].Insert(1, new FontContentTreeView() 
        { 
            Header = "Text_Item_FontContent_Glyphs_UnMapped".GetLocalized(), 
            Icon = "\uF157", 
            Count = (int)LVFont.UnMappedGlyphsCount,
        });

        ContentTreeView[0].Insert(2, new FontContentTreeView() 
        { 
            Header = "Text_Item_FontContent_Glyphs_SingleMapped".GetLocalized(), 
            Icon = "\uF157", 
            Count = (int)LVFont.SingleMappedGlyphsCount,
        });

        ContentTreeView[0].Insert(3, new FontContentTreeView() 
        { 
            Header = "Text_Item_FontContent_Glyphs_MultiMapped".GetLocalized(), 
            Icon = "\uF157", 
            Count = (int)LVFont.MultiMappedGlyphsCount,
        });

        foreach (var range in LVFont.UnicodeRanges)
        {
            ContentTreeView[1].Add(new FontContentTreeView()
            {
                Header = range.Key.Name,
                Icon = "\uE8C1",
                Count = range.Value,
                TooltipText = 
                $"Start: U+{range.Key.Start:X4}\n" +
                $"End: U+{range.Key.End:X4}\n" +
                $"Character Count: {range.Key.End - range.Key.Start + 1}",
            });
        }
    }
}
