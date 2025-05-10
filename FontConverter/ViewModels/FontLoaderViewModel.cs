using LVGLFontConverter.Commands;
using LVGLFontConverter.Contracts.ViewModels;
using LVGLFontConverter.Library;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI;

namespace LVGLFontConverter.ViewModels;

public class FontLoaderViewModel : BaseViewModel
{
    public FontLoaderViewModel(FontConverter fontConverter)
    {
        FontConverter = fontConverter;
        FontLoaderContentDialog = new();
    }

    #region Private Properties
    private const string OkGlypf = "check_circle";
    private const string ErrGlypf = "error";
    private bool _IsApplyButtonEnabled = false;

    private bool _FontIsValidProgressing = true;
    private string _FontIsValidIcon = string.Empty;
    private Brush _FontIsValidIconColor = new SolidColorBrush(Colors.Transparent);
    private Visibility _FontIsValidIconVisibility = Visibility.Collapsed;
    private string _FontNamePath = string.Empty;

    private bool _CountOfTablesProgressing = true;
    private string _CountOfTablesIcon = string.Empty;
    private Brush _CountOfTablesIconColor = new SolidColorBrush(Colors.Transparent);
    private Visibility _CountOfTablesIconVisibility = Visibility.Collapsed;
    private Visibility _CountOfTablesVisibility = Visibility.Collapsed;
    private int _CountOfTables = 0;
    private string _CountOfTablesString = string.Empty;

    private bool _ParsedTablesProgressing = true;
    private string _ParsedTablesIcon = string.Empty;
    private Brush _ParsedTablesIconColor = new SolidColorBrush(Colors.Transparent);
    private Visibility _ParsedTablesIconVisibility = Visibility.Collapsed;
    private Visibility _ParsedTablesVisibility = Visibility.Collapsed;
    private double _ParsedTables = 0.0;
    private string _ParsedTablesReport = string.Empty;

    private bool _CountOfGlyphsProgressing = true;
    private string _CountOfGlyphsIcon = string.Empty;
    private Brush _CountOfGlyphsIconColor = new SolidColorBrush(Colors.Transparent);
    private Visibility _CountOfGlyphsIconVisibility = Visibility.Collapsed;
    private Visibility _CountOfGlyphsVisibility = Visibility.Collapsed;
    private int _CountOfGlyphs = 0;
    private string _CountOfGlyphsString = string.Empty;

    private bool _RenderedGlyphsProgressing = true;
    private string _RenderedGlyphsIcon = string.Empty;
    private Brush _RenderedGlyphsIconColor = new SolidColorBrush(Colors.Transparent);
    private Visibility _RenderedGlyphsIconVisibility = Visibility.Collapsed;
    private Visibility _RenderedGlyphsVisibility = Visibility.Collapsed;
    private double _RenderedGlyphs = 0.0;
    private string _RenderedGlyphsReport = string.Empty;

    private bool _OrganizedGlyphsProgressing = true;
    private string _OrganizedGlyphsIcon = string.Empty;
    private Brush _OrganizedGlyphsIconColor = new SolidColorBrush(Colors.Transparent);
    private Visibility _OrganizedGlyphsIconVisibility = Visibility.Collapsed;
    private Visibility _OrganizedGlyphsVisibility = Visibility.Collapsed;
    private double _OrganizedGlyphs = 0.0;
    private string _OrganizedGlyphsReport = string.Empty;

    private bool _FinalizedFontProgressing = true;
    private string _FinalizedFontIcon = string.Empty;
    private Brush _FinalizedFontIconColor = new SolidColorBrush(Colors.Transparent);
    private Visibility _FinalizedFontIconVisibility = Visibility.Collapsed;
    private Visibility _FinalizedFontVisibility = Visibility.Collapsed;
    private double _FinalizedFont = 0.0;
    private string _FinalizedFontReport = string.Empty;
    #endregion Private Properties

    #region Public Properties
    public FontConverter FontConverter { get; private set; }
    public CancellationTokenSource? FontLoadingCancellationToken { get; set; }
    public ContentDialog FontLoaderContentDialog { get;  set; }

    public bool FontIsValidProgressing
    {
        get { return _FontIsValidProgressing; }
        set { SetProperty(ref _FontIsValidProgressing, value); }
    }
    public string FontIsValidIcon
    {
        get { return _FontIsValidIcon; }
        set { SetProperty(ref _FontIsValidIcon, value); }
    }
    public Brush FontIsValidIconColor
    {
        get { return _FontIsValidIconColor; }
        set { SetProperty(ref _FontIsValidIconColor, value); }
    }
    public Visibility FontIsValidIconVisibility
    {
        get { return _FontIsValidIconVisibility; }
        set { SetProperty(ref _FontIsValidIconVisibility, value); }
    }
    public string FontNamePath
    {
		get { return _FontNamePath; }
		set { SetProperty(ref _FontNamePath, value); }
	}

    public bool CountOfTablesProgressing
    {
        get { return _CountOfTablesProgressing; }
        set { SetProperty(ref _CountOfTablesProgressing, value); }
    }
    public string CountOfTablesIcon
    {
        get { return _CountOfTablesIcon; }
        set { SetProperty(ref _CountOfTablesIcon, value); }
    }
    public Brush CountOfTablesIconColor
    {
        get { return _CountOfTablesIconColor; }
        set { SetProperty(ref _CountOfTablesIconColor, value); }
    }
    public Visibility CountOfTablesIconVisibility
    {
        get { return _CountOfTablesIconVisibility; }
        set { SetProperty(ref _CountOfTablesIconVisibility, value); }
    }
    public Visibility CountOfTablesVisibility
    {
        get { return _CountOfTablesVisibility; }
        set { SetProperty(ref _CountOfTablesVisibility, value); }
    }
    public int CountOfTables
    {
        get { return _CountOfTables; }
        set { SetProperty(ref _CountOfTables, value); }
    }
    public string CountOfTablesString
    {
        get { return _CountOfTablesString; }
        set { SetProperty(ref _CountOfTablesString, value); }
    }

    public bool ParsedTablesProgressing
    {
        get { return _ParsedTablesProgressing; }
        set { SetProperty(ref _ParsedTablesProgressing, value); }
    }
    public string ParsedTablesIcon
    {
        get { return _ParsedTablesIcon; }
        set { SetProperty(ref _ParsedTablesIcon, value); }
    }
    public Brush ParsedTablesIconColor
    {
        get { return _ParsedTablesIconColor; }
        set { SetProperty(ref _ParsedTablesIconColor, value); }
    }
    public Visibility ParsedTablesIconVisibility
    {
        get { return _ParsedTablesIconVisibility; }
        set { SetProperty(ref _ParsedTablesIconVisibility, value); }
    }
    public Visibility ParsedTablesVisibility
    {
        get { return _ParsedTablesVisibility; }
        set { SetProperty(ref _ParsedTablesVisibility, value); }
    }
    public double ParsedTables
    {
        get { return _ParsedTables; }
        set { SetProperty(ref _ParsedTables, value); }
    }
    public string ParsedTablesReport
    {
        get { return _ParsedTablesReport; }
        set { SetProperty(ref _ParsedTablesReport, value); }
    }

    public bool CountOfGlyphsProgressing
    {
        get { return _CountOfGlyphsProgressing; }
        set { SetProperty(ref _CountOfGlyphsProgressing, value); }
    }
    public string CountOfGlyphsIcon
    {
        get { return _CountOfGlyphsIcon; }
        set { SetProperty(ref _CountOfGlyphsIcon, value); }
    }
    public Brush CountOfGlyphsIconColor
    {
        get { return _CountOfGlyphsIconColor; }
        set { SetProperty(ref _CountOfGlyphsIconColor, value); }
    }
    public Visibility CountOfGlyphsIconVisibility
    {
        get { return _CountOfGlyphsIconVisibility; }
        set { SetProperty(ref _CountOfGlyphsIconVisibility, value); }
    }
    public Visibility CountOfGlyphsVisibility
    {
        get { return _CountOfGlyphsVisibility; }
        set { SetProperty(ref _CountOfGlyphsVisibility, value); }
    }
    public int CountOfGlyphs
    {
        get { return _CountOfGlyphs; }
        set { SetProperty(ref _CountOfGlyphs, value); }
    }
    public string CountOfGlyphsString
    {
        get { return _CountOfGlyphsString; }
        set { SetProperty(ref _CountOfGlyphsString, value); }
    }

    public bool RenderedGlyphsProgressing
    {
        get { return _RenderedGlyphsProgressing; }
        set { SetProperty(ref _RenderedGlyphsProgressing, value); }
    }
    public string RenderedGlyphsIcon
    {
        get { return _RenderedGlyphsIcon; }
        set { SetProperty(ref _RenderedGlyphsIcon, value); }
    }
    public Brush RenderedGlyphsIconColor
    {
        get { return _RenderedGlyphsIconColor; }
        set { SetProperty(ref _RenderedGlyphsIconColor, value); }
    }
    public Visibility RenderedGlyphsIconVisibility
    {
        get { return _RenderedGlyphsIconVisibility; }
        set { SetProperty(ref _RenderedGlyphsIconVisibility, value); }
    }
    public Visibility RenderedGlyphsVisibility
    {
        get { return _RenderedGlyphsVisibility; }
        set { SetProperty(ref _RenderedGlyphsVisibility, value); }
    }
    public double RenderedGlyphs
    {
        get { return _RenderedGlyphs; }
        set { SetProperty(ref _RenderedGlyphs, value); }
    }
    public string RenderedGlyphsReport
    {
        get { return _RenderedGlyphsReport; }
        set { SetProperty(ref _RenderedGlyphsReport, value); }
    }

    public bool OrganizedGlyphsProgressing
    {
        get { return _OrganizedGlyphsProgressing; }
        set { SetProperty(ref _OrganizedGlyphsProgressing, value); }
    }
    public string OrganizedGlyphsIcon
    {
        get { return _OrganizedGlyphsIcon; }
        set { SetProperty(ref _OrganizedGlyphsIcon, value); }
    }
    public Brush OrganizedGlyphsIconColor
    {
        get { return _OrganizedGlyphsIconColor; }
        set { SetProperty(ref _OrganizedGlyphsIconColor, value); }
    }
    public Visibility OrganizedGlyphsIconVisibility
    {
        get { return _OrganizedGlyphsIconVisibility; }
        set { SetProperty(ref _OrganizedGlyphsIconVisibility, value); }
    }
    public Visibility OrganizedGlyphsVisibility
    {
        get { return _OrganizedGlyphsVisibility; }
        set { SetProperty(ref _OrganizedGlyphsVisibility, value); }
    }
    public double OrganizedGlyphs
    {
        get { return _OrganizedGlyphs; }
        set { SetProperty(ref _OrganizedGlyphs, value); }
    }
    public string OrganizedGlyphsReport
    {
        get { return _OrganizedGlyphsReport; }
        set { SetProperty(ref _OrganizedGlyphsReport, value); }
    }

    public bool FinalizedFontProgressing
    {
        get { return _FinalizedFontProgressing; }
        set { SetProperty(ref _FinalizedFontProgressing, value); }
    }
    public string FinalizedFontIcon
    {
        get { return _FinalizedFontIcon; }
        set { SetProperty(ref _FinalizedFontIcon, value); }
    }
    public Brush FinalizedFontIconColor
    {
        get { return _FinalizedFontIconColor; }
        set { SetProperty(ref _FinalizedFontIconColor, value); }
    }
    public Visibility FinalizedFontIconVisibility
    {
        get { return _FinalizedFontIconVisibility; }
        set { SetProperty(ref _FinalizedFontIconVisibility, value); }
    }
    public Visibility FinalizedFontVisibility
    {
        get { return _FinalizedFontVisibility; }
        set { SetProperty(ref _FinalizedFontVisibility, value); }
    }
    public double FinalizedFont
    {
        get { return _FinalizedFont; }
        set { SetProperty(ref _FinalizedFont, value); }
    }
    public string FinalizedFontReport
    {
        get { return _FinalizedFontReport; }
        set { SetProperty(ref _FinalizedFontReport, value); }
    }
    #endregion Public Properties

    public async Task ApplyFont()
    {
        await Task.Delay(100);
    }

    public async void StartLoading()
    {
        ClearLoadingData();
        
        var dispatcher = App.MainWindow.DispatcherQueue;
        FontLoadingCancellationToken = new CancellationTokenSource();
        try
        {
            await FontConverter.ValidateFontAsync(FontNamePath, FontLoadingCancellationToken.Token);
            FontIsValidProgressing = false;
            FontIsValidIcon = FontConverter.IsFontValid ? OkGlypf : ErrGlypf;
            FontIsValidIconColor = FontConverter.IsFontValid ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
            FontIsValidIconVisibility = Visibility.Visible;

            if (FontConverter.IsFontValid)
            {
                CountOfTablesVisibility = Visibility.Visible;
                var progressTableList = new Progress<string>(message =>
                {
                    if (message.StartsWith("Processed"))
                    {
                        CountOfTables++;
                        CountOfTablesString = string.Format("{0:N0}", CountOfTables);
                    }
                });
                await FontConverter.GetTablesListAsync(progressTableList, FontLoadingCancellationToken.Token);
                CountOfTablesProgressing = false;
                CountOfTablesIcon = FontConverter.IsTablesListReady ? OkGlypf : ErrGlypf;
                CountOfTablesIconColor = FontConverter.IsTablesListReady ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
                CountOfTablesIconVisibility = Visibility.Visible;

                if (FontConverter.IsTablesListReady)
                {
                    ParsedTablesVisibility = Visibility.Visible;
                    var progressTablesData = new Progress<(string tableName, double percentage)>(report =>
                    {
                        ParsedTables = report.percentage;
                        ParsedTablesReport = $"{report.percentage:F1}%";
                    });
                    await FontConverter.GetTablesDataAsync(progressTablesData, FontLoadingCancellationToken.Token);
                    ParsedTablesProgressing = false;
                    ParsedTablesIcon = FontConverter.IsTablesDataReady ? OkGlypf : ErrGlypf;
                    ParsedTablesIconColor = FontConverter.IsTablesDataReady ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
                    ParsedTablesIconVisibility = Visibility.Visible;

                    if (FontConverter.IsTablesDataReady)
                    {
                        CountOfGlyphsProgressing = false;
                        CountOfGlyphsVisibility = Visibility.Visible;
                        CountOfGlyphs = FontConverter.OpenTypeFont.MaxpTable.NumGlyphs;
                        CountOfGlyphsString = string.Format("{0:N0}", CountOfGlyphs);
                        CountOfGlyphsIcon = FontConverter.IsTablesDataReady ? OkGlypf : ErrGlypf;
                        CountOfGlyphsIconColor = FontConverter.IsTablesDataReady ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
                        CountOfGlyphsIconVisibility = Visibility.Visible;

                        RenderedGlyphsVisibility = Visibility.Visible;
                        var progressRenderedGlyphs = new Progress<(int glyphIndex, double percentage)>(report =>
                        {
                            RenderedGlyphs = report.percentage;
                            RenderedGlyphsReport = $"{report.percentage:F1}%";
                        });
                        await FontConverter.GetGlyphsBitmapAsync(progressRenderedGlyphs, FontLoadingCancellationToken.Token);
                        RenderedGlyphsProgressing = false;
                        RenderedGlyphsIcon = FontConverter.IsGlyphsBitmapReady ? OkGlypf : ErrGlypf;
                        RenderedGlyphsIconColor = FontConverter.IsGlyphsBitmapReady ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
                        RenderedGlyphsIconVisibility = Visibility.Visible;

                        if (FontConverter.IsGlyphsBitmapReady)
                        {
                            OrganizedGlyphsVisibility = Visibility.Visible;
                            var progressOrganizedGlyphs = new Progress<(int glyphIndex, double percentage)>(report =>
                            {
                                OrganizedGlyphs = report.percentage;
                                OrganizedGlyphsReport = $"{report.percentage:F1}%";
                            });
                            await FontConverter.GetGlyphsDataAsync(progressOrganizedGlyphs, FontLoadingCancellationToken.Token);
                            OrganizedGlyphsProgressing = false;
                            OrganizedGlyphsIcon = FontConverter.IsGlyphsDataReady ? OkGlypf : ErrGlypf;
                            OrganizedGlyphsIconColor = FontConverter.IsGlyphsDataReady ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
                            OrganizedGlyphsIconVisibility = Visibility.Visible;

                            if (FontConverter.IsGlyphsDataReady)
                            {
                                FinalizedFontVisibility = Visibility.Visible;
                                var progressFinalizedFont = new Progress<double>(report =>
                                {
                                    FinalizedFont = report;
                                    FinalizedFontReport = $"{report:F1}%";
                                });
                                await FontConverter.FinalizeFontDataAsync(progressFinalizedFont, FontLoadingCancellationToken.Token);
                                FinalizedFontProgressing = false;
                                FinalizedFontIcon = FontConverter.IsFontConverted ? OkGlypf : ErrGlypf;
                                FinalizedFontIconColor = FontConverter.IsFontConverted ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
                                FinalizedFontIconVisibility = Visibility.Visible;

                                if (FontConverter.IsFontConverted && FontLoaderContentDialog!=null)
                                {
                                    FontLoaderContentDialog.IsPrimaryButtonEnabled = true;
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            
        }
        catch (Exception)
        {
            
        }
        finally
        {
            //FontLoadingCancellationToken?.Dispose();
            //FontLoadingCancellationToken = null;
        }
    }

    public void ClearLoadingData()
    {
        FontIsValidProgressing = true;
        FontIsValidIcon = string.Empty;
        FontIsValidIconColor = new SolidColorBrush(Colors.Transparent);
        FontIsValidIconVisibility = Visibility.Collapsed;
        CountOfTablesIconVisibility = Visibility.Collapsed;

        CountOfTablesProgressing = true;
        CountOfTablesIcon = string.Empty;
        CountOfTablesIconColor = new SolidColorBrush(Colors.Transparent);
        CountOfTablesVisibility = Visibility.Collapsed;
        CountOfTables = 0;
        CountOfTablesString = string.Empty;

        ParsedTablesProgressing = true;
        ParsedTablesIcon = string.Empty;
        ParsedTablesIconColor = new SolidColorBrush(Colors.Transparent);
        ParsedTablesIconVisibility = Visibility.Collapsed;
        ParsedTablesVisibility = Visibility.Collapsed;
        ParsedTables = 0.0;
        ParsedTablesReport = string.Empty;

        CountOfGlyphsProgressing = true;
        CountOfGlyphsIcon = string.Empty;
        CountOfGlyphsIconColor = new SolidColorBrush(Colors.Transparent);
        CountOfGlyphsIconVisibility = Visibility.Collapsed;
        CountOfGlyphsVisibility = Visibility.Collapsed;
        CountOfGlyphs = 0;
        CountOfGlyphsString = string.Empty;

        RenderedGlyphsProgressing = true;
        RenderedGlyphsIcon = string.Empty;
        RenderedGlyphsIconColor = new SolidColorBrush(Colors.Transparent);
        RenderedGlyphsIconVisibility = Visibility.Collapsed;
        RenderedGlyphsVisibility = Visibility.Collapsed;
        RenderedGlyphs = 0.0;
        RenderedGlyphsReport = string.Empty;

        OrganizedGlyphsProgressing = true;
        OrganizedGlyphsIcon = string.Empty;
        OrganizedGlyphsIconColor = new SolidColorBrush(Colors.Transparent);
        OrganizedGlyphsIconVisibility = Visibility.Collapsed;
        OrganizedGlyphsVisibility = Visibility.Collapsed;
        OrganizedGlyphs = 0.0;
        OrganizedGlyphsReport = string.Empty;

        FinalizedFontProgressing = true;
        FinalizedFontIcon = string.Empty;
        FinalizedFontIconColor = new SolidColorBrush(Colors.Transparent);
        FinalizedFontIconVisibility = Visibility.Collapsed;
        FinalizedFontVisibility = Visibility.Collapsed;
        FinalizedFont = 0.0;
        FinalizedFontReport = string.Empty;
    }
}
