using LVGLFontConverter.Library;
using LVGLFontConverter.Library.Models;
using LVGLFontConverter.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using WinRT.Interop;
using Microsoft.UI.Windowing;
using LVGLFontConverter.Helpers;
using System;
using System.Runtime.InteropServices;
using Windows.UI.WindowManagement;


namespace LVGLFontConverter.Views;


public sealed partial class MainWindow : Window
{
    public MainViewModel ViewModel { get; }
    
    public delegate int SUBCLASSPROC(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam, IntPtr uIdSubclass, uint dwRefData);
    [StructLayout(LayoutKind.Sequential)]
    public struct MINMAXINFO
    {
        public System.Drawing.Point ptReserved;
        public System.Drawing.Point ptMaxSize;
        public System.Drawing.Point ptMaxPosition;
        public System.Drawing.Point ptMinTrackSize;
        public System.Drawing.Point ptMaxTrackSize;
    }
    [DllImport("Comctl32.dll", SetLastError = true)]
    public static extern int DefSubclassProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);
    [DllImport("Comctl32.dll", SetLastError = true)]
    public static extern bool SetWindowSubclass(IntPtr hWnd, SUBCLASSPROC pfnSubclass, uint uIdSubclass, uint dwRefData);
    private SUBCLASSPROC SubClassDelegate;
    private const int WM_GETMINMAXINFO = 0x0024;
    

    public MainWindow()
    {
        ViewModel = App.GetService<MainViewModel>();

        InitializeComponent();

        App._HWND = WindowNative.GetWindowHandle(this);
        App._WindowId = Win32Interop.GetWindowIdFromWindow(App._HWND);
        App._AppWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(App._WindowId);
        App._AppWindow.Resize(new Windows.Graphics.SizeInt32(1200, 800));
        App._AppWindow.SetIcon("Assets/FontConverter.ico");
        SubClassDelegate = new SUBCLASSPROC(WindowSubClass);
        bool bReturn = SetWindowSubclass(App._HWND, SubClassDelegate, 0, 0);
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(CustomTitleBar);
        double titleBarHeight = AppWindow.TitleBar.Height;
        ViewModel.TitleBarHight = new(titleBarHeight);

        ViewModel.NavigationService.Frame = NavigationFrame;
    }

    private int WindowSubClass(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam, IntPtr uIdSubclass, uint dwRefData)
    {
        switch (uMsg)
        {
            case WM_GETMINMAXINFO:
                MINMAXINFO mmi = Marshal.PtrToStructure<MINMAXINFO>(lParam);
                mmi.ptMinTrackSize.X = 80;
                mmi.ptMinTrackSize.Y = 500;
                Marshal.StructureToPtr(mmi, lParam, false);
                return 0;
        }
        return DefSubclassProc(hWnd, uMsg, wParam, lParam);
    }
}
 