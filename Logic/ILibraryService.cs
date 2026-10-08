using Data;

namespace Logic;

public interface ILibraryService
{
    event EventHandler<LibraryChangedEventArgs>? LibraryChanged;

    // Core operations
    Task<IUser> RegisterUserAsync(string name, string email);
    Task<IBook> AddBookToCatalogAsync(string title, string author, int year);
    Task<bool> BorrowBookAsync(int userId, int bookId);
    Task<bool> ReturnBookAsync(int userId, int bookId);
    Task<bool> IsBookAvailableAsync(int bookId);

    // Queries
    Task<IEnumerable<IEvent>> GetHistoryAsync();
    Task<IEnumerable<IUser>> GetAllUsersAsync();
    Task<IEnumerable<IBook>> GetAllBooksAsync();
    Task<IUser?> GetUserAsync(int id);
    Task<IBook?> GetBookAsync(int id);

    // CRUD extensions
    Task UpdateUserAsync(int id, string name, string email);
    Task DeleteUserAsync(int id);
    Task UpdateBookAsync(int id, string title, string author, int year);
    Task DeleteBookAsync(int id);
}
