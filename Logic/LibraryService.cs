using Data;

namespace Logic;

public class LibraryService : ILibraryService
{
    private readonly IDataRepository _repo;

    public event EventHandler<LibraryChangedEventArgs>? LibraryChanged;

    public LibraryService(IDataRepository repo)
    {
        _repo = repo;
    }

    public async Task<IUser> RegisterUserAsync(string name, string email)
    {
        var user = await _repo.AddUserAsync(name, email);
        LibraryChanged?.Invoke(this, new LibraryChangedEventArgs(LibraryChangeType.UserAdded, user.Id));
        return user;
    }

    public async Task<IBook> AddBookToCatalogAsync(string title, string author, int year)
    {
        var book = await _repo.AddBookAsync(title, author, year);
        LibraryChanged?.Invoke(this, new LibraryChangedEventArgs(LibraryChangeType.BookAdded, book.Id));
        return book;
    }

    public async Task<bool> BorrowBookAsync(int userId, int bookId)
    {
        var user = await _repo.GetUserAsync(userId);
        var book = await _repo.GetBookAsync(bookId);
        if (user == null || book == null) return false;
        if (!await IsBookAvailableAsync(bookId)) return false;

        await _repo.UpdateStateAsync(bookId, false);
        await _repo.AddEventAsync(userId, bookId, DateTime.Now, "Borrow");
        LibraryChanged?.Invoke(this, new LibraryChangedEventArgs(LibraryChangeType.BookBorrowed, bookId));
        return true;
    }

    public async Task<bool> ReturnBookAsync(int userId, int bookId)
    {
        var user = await _repo.GetUserAsync(userId);
        var book = await _repo.GetBookAsync(bookId);
        if (user == null || book == null) return false;

        await _repo.UpdateStateAsync(bookId, true);
        await _repo.AddEventAsync(userId, bookId, DateTime.Now, "Return");
        LibraryChanged?.Invoke(this, new LibraryChangedEventArgs(LibraryChangeType.BookReturned, bookId));
        return true;
    }

    public async Task<bool> IsBookAvailableAsync(int bookId)
    {
        var state = await _repo.GetStateAsync(bookId);
        return state?.IsAvailable ?? false;
    }

    public Task<IEnumerable<IEvent>> GetHistoryAsync() => _repo.GetAllEventsAsync();
    public Task<IEnumerable<IUser>> GetAllUsersAsync() => _repo.GetAllUsersAsync();
    public Task<IEnumerable<IBook>> GetAllBooksAsync() => _repo.GetAllBooksAsync();
    public Task<IUser?> GetUserAsync(int id) => _repo.GetUserAsync(id);
    public Task<IBook?> GetBookAsync(int id) => _repo.GetBookAsync(id);

    public async Task UpdateUserAsync(int id, string name, string email)
    {
        await _repo.UpdateUserAsync(id, name, email);
        LibraryChanged?.Invoke(this, new LibraryChangedEventArgs(LibraryChangeType.UserUpdated, id));
    }

    public async Task DeleteUserAsync(int id)
    {
        await _repo.DeleteUserAsync(id);
        LibraryChanged?.Invoke(this, new LibraryChangedEventArgs(LibraryChangeType.UserDeleted, id));
    }

    public async Task UpdateBookAsync(int id, string title, string author, int year)
    {
        await _repo.UpdateBookAsync(id, title, author, year);
        LibraryChanged?.Invoke(this, new LibraryChangedEventArgs(LibraryChangeType.BookUpdated, id));
    }

    public async Task DeleteBookAsync(int id)
    {
        await _repo.DeleteBookAsync(id);
        LibraryChanged?.Invoke(this, new LibraryChangedEventArgs(LibraryChangeType.BookDeleted, id));
    }
}
