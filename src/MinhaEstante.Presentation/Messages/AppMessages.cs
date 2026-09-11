using MinhaEstante.Presentation.ViewModels;

namespace MinhaEstante.Presentation.Messages;

public sealed record NavigateMessage(ViewModelBase ViewModel);

public sealed record NavigateHomeMessage;

public sealed record LibraryChangedMessage;

public sealed record LibrarySettingsChangedMessage;

public sealed record EditBookMessage(Guid BookId);

public sealed record ReadingProgressChangedMessage(Guid BookId, int CurrentPage);

public sealed record ClearReaderCacheMessage;