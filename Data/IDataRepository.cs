namespace Data;

public interface IDataRepository
{
    // Users
    Task<IUser> AddUserAsync(string name, string email);
    Task<IUser?> GetUserAsync(int id);
    Task<IEnumerable<IUser>> GetAllUsersAsync();
    Task UpdateUserAsync(int id, string name, string email);
    Task DeleteUserAsync(int id);

    // Books
    Task<IBook> AddBookAsync(string title, string author, int year);
    Task<IBook?> GetBookAsync(int id);
    Task<IEnumerable<IBook>> GetAllBooksAsync();
    Task UpdateBookAsync(int id, string title, string author, int year);
    Task DeleteBookAsync(int id);

    // State
    Task<IState?> GetStateAsync(int bookId);
    Task UpdateStateAsync(int bookId, bool isAvailable);

    // Events
    Task<IEvent> AddEventAsync(int userId, int bookId, DateTime date, string type);
    Task<IEnumerable<IEvent>> GetAllEventsAsync();
}
