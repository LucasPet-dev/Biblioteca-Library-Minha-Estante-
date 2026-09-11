using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MinhaEstante.Application.Interfaces;
using MinhaEstante.Presentation.Messages;

namespace MinhaEstante.Presentation.ViewModels;

public partial class BookDetailsViewModel : ViewModelBase
{
    private readonly IOpenBookUseCase _openBook;
    private readonly IEditBookMetadataUseCase _editMetadata;

    private Guid _bookId;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _author = string.Empty;

    [ObservableProperty]
    private string _genre = string.Empty;

    public BookDetailsViewModel(IOpenBookUseCase openBook, IEditBookMetadataUseCase editMetadata)
    {
        _openBook = openBook;
        _editMetadata = editMetadata;
    }

    public void SetBookId(Guid bookId) => _bookId = bookId;

    public async Task LoadAsync()
    {
        var result = await _openBook.ExecuteAsync(_bookId);

        Title = result.Book.Title;
        Author = result.Book.Author ?? string.Empty;
        Genre = result.Book.Genre ?? string.Empty;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        await _editMetadata.ExecuteAsync(
            _bookId,
            Title,
            string.IsNullOrWhiteSpace(Author) ? null : Author,
            string.IsNullOrWhiteSpace(Genre) ? null : Genre);

        WeakReferenceMessenger.Default.Send(new LibraryChangedMessage());
        WeakReferenceMessenger.Default.Send(new NavigateHomeMessage());
    }

    [RelayCommand]
    private void Cancel() => WeakReferenceMessenger.Default.Send(new NavigateHomeMessage());
}