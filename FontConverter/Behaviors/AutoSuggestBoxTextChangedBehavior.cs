using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;
using System;
using System.Windows.Input;

namespace LVGLFontConverter.Behaviors;

public class AutoSuggestBoxCommandBehavior : Behavior<AutoSuggestBox>
{
    public ICommand? Command
    {
        get => (ICommand?)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(AutoSuggestBoxCommandBehavior), new PropertyMetadata(null));

    public static readonly DependencyProperty SuggestionChosenCommandProperty =
        DependencyProperty.Register(nameof(SuggestionChosenCommand), typeof(ICommand), typeof(AutoSuggestBoxCommandBehavior), null);

    public static readonly DependencyProperty QuerySubmittedCommandProperty =
        DependencyProperty.Register(nameof(QuerySubmittedCommand), typeof(ICommand), typeof(AutoSuggestBoxCommandBehavior), null);

    public ICommand? SuggestionChosenCommand
    {
        get => (ICommand?)GetValue(SuggestionChosenCommandProperty);
        set => SetValue(SuggestionChosenCommandProperty, value);
    }

    public ICommand? QuerySubmittedCommand
    {
        get => (ICommand?)GetValue(QuerySubmittedCommandProperty);
        set => SetValue(QuerySubmittedCommandProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.TextChanged += OnTextChanged;
        AssociatedObject.GotFocus += OnGotFocus;
        AssociatedObject.SuggestionChosen += OnSuggestionChosen;
        AssociatedObject.QuerySubmitted += OnQuerySubmitted;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.TextChanged -= OnTextChanged;
        AssociatedObject.GotFocus -= OnGotFocus;
        AssociatedObject.SuggestionChosen -= OnSuggestionChosen;
        AssociatedObject.QuerySubmitted -= OnQuerySubmitted;
    }

    private void OnTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            TryExecuteCommand(sender.Text);
        }
    }

    private void OnGotFocus(object sender, RoutedEventArgs e)
    {
        if (sender is AutoSuggestBox box)
        {
            TryExecuteCommand(box.Text);
        }
    }

    private void OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        SuggestionChosenCommand?.Execute(args.SelectedItem);
    }

    private void OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        QuerySubmittedCommand?.Execute(args.QueryText);
    }

    private void TryExecuteCommand(string text)
    {
        if (AssociatedObject == null ||
            AssociatedObject.DispatcherQueue == null ||
            AssociatedObject.DispatcherQueue.HasThreadAccess == false)
            return;

        if (Command?.CanExecute(text) == true)
        {
            Command.Execute(text);
        }
    }
}