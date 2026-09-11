using System.Collections.ObjectModel;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MinhaEstante.Application.Interfaces;
using MinhaEstante.Application.Models;
using MinhaEstante.Domain.Enums;
using MinhaEstante.Presentation.Messages;
using MinhaEstante.Presentation.Services;

namespace MinhaEstante.Presentation.ViewModels;

public partial class LibraryViewModel : ViewModelBase
{
    private readonly IGetLibraryUseCase _getLibrary;
    private readonly IRemoveBookUseCase _removeBook;
    private readonly IGetSettingsUseCase _getSettings;
    private readonly IDialogService _dialogService;
    private readonly ImportViewModel _importViewModel;
    private readonly Func<Guid, ReaderViewModel> _readerFactory;
    private readonly Func<Guid, BookDetailsViewModel> _detailsFactory;

    public ObservableCollection<BookCardViewModel> Books { get; } = new();

    [ObservableProperty]
    private bool _isEmpty = true;

    public LibraryViewModel(
        IGetLibraryUseCase getLibrary,
        IRemoveBookUseCase removeBook,
        IGetSettingsUseCase getSettings,
        IDialogService dialogService,
        ImportViewModel importViewModel,
        Func<Guid, ReaderViewModel> readerFactory,
        Func<Guid, BookDetailsViewModel> detailsFactory)
    {
        _getLibrary = getLibrary;
        _removeBook = removeBook;
        _getSettings = getSettings;
        _dialogService = dialogService;
        _importViewModel = importViewModel;
        _readerFactory = readerFactory;
        _detailsFactory = detailsFactory;

        WeakReferenceMessenger.Default.Register<LibraryViewModel, LibraryChangedMessage>(this, (r, m) => { _ = r.RefreshAsync(); });
        WeakReferenceMessenger.Default.Register<LibraryViewModel, LibrarySettingsChangedMessage>(this, (r, m) => { _ = r.RefreshAsync(); });
        WeakReferenceMessenger.Default.Register<LibraryViewModel, ReadingProgressChangedMessage>(this, (r, m) => r.ApplyProgress(m));
    }

    public Task InitializeAsync() => RefreshAsync();

    [RelayCommand]
    private void NavigateToImport()
    {
        WeakReferenceMessenger.Default.Send(new NavigateMessage(_importViewModel));
    }

    public async Task RefreshAsync()
    {
        var items = await _getLibrary.ExecuteAsync();
        var settings = await _getSettings.ExecuteAsync();

        var ordered = Sort(items, settings.SortOrder).ToList();

        Books.Clear();

        foreach (var item in ordered)
        {
            Books.Add(new BookCardViewModel(
                item.Book.Id,
                item.Book.Title,
                LoadCover(item.Book.CoverImagePath),
                item.CurrentPage,
                item.Book.TotalPages,
                onOpen: OpenBook,
                onEdit: EditBook,
                onRemove: RemoveBook,
                author: item.Book.Author,
                genre: item.Book.Genre));
        }

        IsEmpty = Books.Count == 0;
    }

    private static IEnumerable<LibraryItem> Sort(IReadOnlyList<LibraryItem> items, LibrarySortOrder sortOrder)
    {
        return sortOrder switch
        {
            LibrarySortOrder.Author => items
                .OrderBy(i => i.Book.Author is null)
                .ThenBy(i => i.Book.Author, StringComparer.CurrentCultureIgnoreCase),
            LibrarySortOrder.AddedAt => items.OrderByDescending(i => i.Book.AddedAt),
            LibrarySortOrder.LastReadAt => items
                .OrderBy(i => i.LastReadAt is null)
                .ThenByDescending(i => i.LastReadAt),
            _ => items.OrderBy(i => i.Book.Title, StringComparer.CurrentCultureIgnoreCase),
        };
    }

    private void ApplyProgress(ReadingProgressChangedMessage message)
    {
        var card = Books.FirstOrDefault(b => b.Id == message.BookId);

        if (card is null)
        {
            return;
        }

        card.CurrentPage = message.CurrentPage;
    }

    private void OpenBook(Guid bookId)
    {
        var reader = _readerFactory(bookId);
        WeakReferenceMessenger.Default.Send(new NavigateMessage(reader));
        _ = reader.LoadAsync();
    }

    private async void EditBook(Guid bookId)
    {
        var details = _detailsFactory(bookId);
        await details.LoadAsync();
        WeakReferenceMessenger.Default.Send(new NavigateMessage(details));
    }

    private async void RemoveBook(Guid bookId)
    {
        var title = Books.FirstOrDefault(b => b.Id == bookId)?.Title ?? string.Empty;

        var choice = await _dialogService.ConfirmRemoveAsync(title);

        if (choice is null)
        {
            return;
        }

        await _removeBook.ExecuteAsync(bookId, choice.DeleteFile);
        await RefreshAsync();
    }

    private static Bitmap? LoadCover(string? coverImagePath)
    {
        if (string.IsNullOrWhiteSpace(coverImagePath) || !File.Exists(coverImagePath))
        {
            return null;
        }

        return new Bitmap(coverImagePath);
    }
}