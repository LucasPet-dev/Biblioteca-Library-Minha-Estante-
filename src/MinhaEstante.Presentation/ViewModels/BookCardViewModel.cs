using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MinhaEstante.Presentation.ViewModels;

public partial class BookCardViewModel : ViewModelBase
{
    public Guid Id { get; }

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private Bitmap? _cover;

    [ObservableProperty]
    private int _currentPage;

    [ObservableProperty]
    private int _totalPages;

    [ObservableProperty]
    private double _progressPercent;

    [ObservableProperty]
    private string _progressLabel = string.Empty;

    public string? Author { get; }

    public string? Genre { get; }

    public IRelayCommand OpenCommand { get; }

    public IRelayCommand EditCommand { get; }

    public IRelayCommand RemoveCommand { get; }

    public BookCardViewModel(
        Guid id,
        string title,
        Bitmap? cover,
        int currentPage,
        int totalPages,
        Action<Guid>? onOpen = null,
        Action<Guid>? onEdit = null,
        Action<Guid>? onRemove = null,
        string? author = null,
        string? genre = null)
    {
        Id = id;
        _title = title;
        _cover = cover;
        _currentPage = currentPage;
        _totalPages = totalPages;
        Author = author;
        Genre = genre;

        OpenCommand = new RelayCommand(() => onOpen?.Invoke(Id));
        EditCommand = new RelayCommand(() => onEdit?.Invoke(Id));
        RemoveCommand = new RelayCommand(() => onRemove?.Invoke(Id));

        RecomputeProgress();
    }

    partial void OnCurrentPageChanged(int value) => RecomputeProgress();

    partial void OnTotalPagesChanged(int value) => RecomputeProgress();

    private void RecomputeProgress()
    {
        ProgressPercent = TotalPages > 0
            ? Math.Clamp(CurrentPage / (double)TotalPages * 100.0, 0, 100)
            : 0;

        ProgressLabel = $"{CurrentPage}/{TotalPages}";
    }
}