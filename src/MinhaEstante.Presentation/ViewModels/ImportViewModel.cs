using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MinhaEstante.Application.Interfaces;
using MinhaEstante.Presentation.Messages;

namespace MinhaEstante.Presentation.ViewModels;

public partial class ImportViewModel : ViewModelBase
{
    private readonly IImportBookUseCase _importBook;

    [ObservableProperty]
    private string _sourcePath = string.Empty;

    [ObservableProperty]
    private bool _isImporting;

    [ObservableProperty]
    private string _status = string.Empty;

    public ImportViewModel(IImportBookUseCase importBook)
    {
        _importBook = importBook;
    }

    public void SetSourcePath(string path) => SourcePath = path;

    [RelayCommand]
    private async Task ImportAsync()
    {
        if (string.IsNullOrWhiteSpace(SourcePath))
        {
            Status = "Selecione um arquivo PDF ou CBZ.";
            return;
        }

        IsImporting = true;
        Status = "Importando...";

        try
        {
            var book = await _importBook.ExecuteAsync(SourcePath);
            Status = $"'{book.Title}' importado com sucesso.";

            WeakReferenceMessenger.Default.Send(new LibraryChangedMessage());
            WeakReferenceMessenger.Default.Send(new NavigateHomeMessage());
        }
        catch (Exception ex)
        {
            Status = $"Erro ao importar: {ex.Message}";
        }
        finally
        {
            IsImporting = false;
        }
    }

    [RelayCommand]
    private void Cancel() => WeakReferenceMessenger.Default.Send(new NavigateHomeMessage());
}