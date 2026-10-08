using Logic;

namespace Model;

// Thin data-holder that the ViewModel binds against.
// Performs all data operations through ILibraryService.
public class LibraryModel
{
    private readonly ILibraryService _service;

    public LibraryModel(ILibraryService service)
    {
        _service = service;
        _service.LibraryChanged += (_, args) => LibraryChanged?.Invoke(this, args);
    }

    public event EventHandler<LibraryChangedEventArgs>? LibraryChanged;

    public async Task<IEnumerable<BookModel>> GetBooksAsync()
    {
        var books = await _service.GetAllBooksAsync();
        var result = new List<BookModel>();
        foreach (var book in books)
        {
            var isAvailable = await _service.IsBookAvailableAsync(book.Id);
            result.Add(BookModel.FromBook(book, isAvailable));
        }
        return result;
    }

    public async Task<IEnumerable<UserModel>> GetUsersAsync()
    {
        var users = await _service.GetAllUsersAsync();
        return users.Select(UserModel.FromUser);
    }

    public async Task<IEnumerable<EventModel>> GetEventsAsync()
    {
        var events = await _service.GetHistoryAsync();
        return events.Select(EventModel.FromEvent);
    }

    public Task<Data.IUser> AddUserAsync(string name, string email) =>
        _service.RegisterUserAsync(name, email);

    public Task UpdateUserAsync(int id, string name, string email) =>
        _service.UpdateUserAsync(id, name, email);

    public Task DeleteUserAsync(int id) =>
        _service.DeleteUserAsync(id);

    public Task<Data.IBook> AddBookAsync(string title, string author, int year) =>
        _service.AddBookToCatalogAsync(title, author, year);

    public Task UpdateBookAsync(int id, string title, string author, int year) =>
        _service.UpdateBookAsync(id, title, author, year);

    public Task DeleteBookAsync(int id) =>
        _service.DeleteBookAsync(id);

    public Task<bool> BorrowBookAsync(int userId, int bookId) =>
        _service.BorrowBookAsync(userId, bookId);

    public Task<bool> ReturnBookAsync(int userId, int bookId) =>
        _service.ReturnBookAsync(userId, bookId);
}
