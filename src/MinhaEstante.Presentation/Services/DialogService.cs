using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using MinhaEstante.Presentation.Views.Dialogs;

namespace MinhaEstante.Presentation.Services;

public sealed record RemoveBookResult(bool DeleteFile);

public interface IDialogService
{
    Task<RemoveBookResult?> ConfirmRemoveAsync(string bookTitle);
}

public sealed class DialogService : IDialogService
{
    public async Task<RemoveBookResult?> ConfirmRemoveAsync(string bookTitle)
    {
        if (Avalonia.Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop
            || desktop.MainWindow is null)
        {
            return null;
        }

        var dialog = new ConfirmRemoveDialog(bookTitle);
        return await dialog.ShowDialog<RemoveBookResult?>(desktop.MainWindow);
    }
}