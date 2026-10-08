// In-memory async IDataRepository for Logic layer unit tests.
// Production code uses EfDataRepository against SQL Server.
using Data;

namespace Tests.Logic;

internal class FakeDataRepository : IDataRepository
{
    private readonly List<User> _users = new();
    private readonly List<Book> _books = new();
    private readonly List<State> _states = new();
    private readonly List<LibraryEvent> _events = new();

    private int _nextUserId = 1;
    private int _nextBookId = 1;
    private int _nextStateId = 1;
    private int _nextEventId = 1;

    public Task<IUser> AddUserAsync(string name, string email)
    {
        var u = new User { Id = _nextUserId++, Name = name, Email = email };
        _users.Add(u);
        return Task.FromResult<IUser>(u);
    }

    public Task<IUser?> GetUserAsync(int id) =>
        Task.FromResult<IUser?>(_users.FirstOrDefault(u => u.Id == id));

    public Task<IEnumerable<IUser>> GetAllUsersAsync() =>
        Task.FromResult<IEnumerable<IUser>>(_users.Cast<IUser>().ToList());

    public Task UpdateUserAsync(int id, string name, string email)
    {
        var u = _users.First(x => x.Id == id);
        u.Name = name; u.Email = email;
        return Task.CompletedTask;
    }

    public Task DeleteUserAsync(int id)
    {
        _users.RemoveAll(u => u.Id == id);
        return Task.CompletedTask;
    }

    public Task<IBook> AddBookAsync(string title, string author, int year)
    {
        var b = new Book { Id = _nextBookId++, Title = title, Author = author, Year = year };
        var s = new State { Id = _nextStateId++, BookId = b.Id, Book = b, IsAvailable = true };
        _books.Add(b);
        _states.Add(s);
        return Task.FromResult<IBook>(b);
    }

    public Task<IBook?> GetBookAsync(int id) =>
        Task.FromResult<IBook?>(_books.FirstOrDefault(b => b.Id == id));

    public Task<IEnumerable<IBook>> GetAllBooksAsync() =>
        Task.FromResult<IEnumerable<IBook>>(_books.Cast<IBook>().ToList());

    public Task UpdateBookAsync(int id, string title, string author, int year)
    {
        var b = _books.First(x => x.Id == id);
        b.Title = title; b.Author = author; b.Year = year;
        return Task.CompletedTask;
    }

    public Task DeleteBookAsync(int id)
    {
        _books.RemoveAll(b => b.Id == id);
        _states.RemoveAll(s => s.BookId == id);
        return Task.CompletedTask;
    }

    public Task<IState?> GetStateAsync(int bookId) =>
        Task.FromResult<IState?>(_states.FirstOrDefault(s => s.BookId == bookId));

    public Task UpdateStateAsync(int bookId, bool isAvailable)
    {
        var s = _states.First(x => x.BookId == bookId);
        s.IsAvailable = isAvailable;
        return Task.CompletedTask;
    }

    public Task<IEvent> AddEventAsync(int userId, int bookId, DateTime date, string type)
    {
        var user = _users.First(u => u.Id == userId);
        var book = _books.First(b => b.Id == bookId);
        var ev = new LibraryEvent { Id = _nextEventId++, UserId = userId, BookId = bookId, User = user, Book = book, Date = date, Type = type };
        _events.Add(ev);
        return Task.FromResult<IEvent>(ev);
    }

    public Task<IEnumerable<IEvent>> GetAllEventsAsync() =>
        Task.FromResult<IEnumerable<IEvent>>(_events.Cast<IEvent>().ToList());
}
