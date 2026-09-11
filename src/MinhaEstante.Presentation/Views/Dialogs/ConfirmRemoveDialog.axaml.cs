using Avalonia.Controls;
using Avalonia.Interactivity;
using MinhaEstante.Presentation.Resources;
using MinhaEstante.Presentation.Services;

namespace MinhaEstante.Presentation.Views.Dialogs;

public partial class ConfirmRemoveDialog : Window
{
    public ConfirmRemoveDialog()
    {
        InitializeComponent();
    }

    public ConfirmRemoveDialog(string bookTitle)
        : this()
    {
        QuestionText.Text = string.Format(Strings.RemoveBookQuestion, bookTitle);
    }

    private void OnConfirm(object? sender, RoutedEventArgs e)
    {
        Close(new RemoveBookResult(DeleteFileCheck.IsChecked == true));
    }

    private void OnCancel(object? sender, RoutedEventArgs e)
    {
        Close((RemoveBookResult?)null);
    }
}