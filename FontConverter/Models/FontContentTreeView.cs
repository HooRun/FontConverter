using LVGLFontConverter.Commands;
using LVGLFontConverter.ViewModels;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LVGLFontConverter.Models;

public class FontContentTreeView : ObservableCollection<FontContentTreeView>, INotifyPropertyChanged
{
    public FontContentTreeView()
    {
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? name = null)
    {
        if (Equals(storage, value))
            return false;

        storage = value;
        OnPropertyChanged(name);
        return true;
    }

    private string _Header = string.Empty;
	public string Header
    {
		get { return _Header; }
		set { SetProperty(ref _Header, value); }
	}

	private string _Icon = string.Empty;
	public string Icon
	{
		get { return _Icon; }
		set { SetProperty(ref _Icon, value); }
	}

	private int _Count = 0;
	public int Count
	{
		get { return _Count; }
		set { SetProperty(ref _Count, value); }
	}

	private string _TooltipText = string.Empty;
	public string TooltipText
    {
		get { return _TooltipText; }
		set { SetProperty(ref _TooltipText, value); }
	}

	private bool _IsSelected;
	public bool IsSelected
    {
		get { return _IsSelected; }
		set { SetProperty(ref _IsSelected, value); }
	}


	//public AsyncRelayCommand<string>? FilterCommand { get; }
}

