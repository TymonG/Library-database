using Microsoft.EntityFrameworkCore;

namespace Data;

public class EfDataRepository : IDataRepository
{
    private readonly LibraryDbContext _context;

    public EfDataRepository(LibraryDbContext context)
    {
        _context = context;
    }

    // ── Users ──────────────────────────────────────────────────────────

    public async Task<IUser> AddUserAsync(string name, string email)
    {
        var user = new User { Name = name, Email = email };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    // Method syntax — direct key lookup
    public async Task<IUser?> GetUserAsync(int id) =>
        await _context.Users.FindAsync(id);

    // Query syntax — demonstrates required LINQ query form
    public async Task<IEnumerable<IUser>> GetAllUsersAsync()
    {
        var users = await (from u in _context.Users
                           orderby u.Name
                           select u).ToListAsync();
        return users;
    }

    public async Task UpdateUserAsync(int id, string name, string email)
    {
        var user = await _context.Users.FindAsync(id)
                   ?? throw new InvalidOperationException($"User {id} not found.");
        user.Name = name;
        user.Email = email;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id)
                   ?? throw new InvalidOperationException($"User {id} not found.");
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    // ── Books ──────────────────────────────────────────────────────────

    public async Task<IBook> AddBookAsync(string title, string author, int year)
    {
        var book = new Book { Title = title, Author = author, Year = year };
        var state = new State { Book = book, IsAvailable = true };
        _context.Books.Add(book);
        _context.States.Add(state);
        await _context.SaveChangesAsync();
        return book;
    }

    public async Task<IBook?> GetBookAsync(int id) =>
        await _context.Books.FindAsync(id);

    // Method syntax — demonstrates required LINQ method form
    public async Task<IEnumerable<IBook>> GetAllBooksAsync() =>
        await _context.Books
            .OrderBy(b => b.Title)
            .ToListAsync();

    public async Task UpdateBookAsync(int id, string title, string author, int year)
    {
        var book = await _context.Books.FindAsync(id)
                   ?? throw new InvalidOperationException($"Book {id} not found.");
        book.Title = title;
        book.Author = author;
        book.Year = year;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteBookAsync(int id)
    {
        var book = await _context.Books.FindAsync(id)
                   ?? throw new InvalidOperationException($"Book {id} not found.");
        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
    }

    // ── State ──────────────────────────────────────────────────────────

    public async Task<IState?> GetStateAsync(int bookId) =>
        await _context.States.FirstOrDefaultAsync(s => s.BookId == bookId);

    public async Task UpdateStateAsync(int bookId, bool isAvailable)
    {
        var state = await _context.States.FirstOrDefaultAsync(s => s.BookId == bookId)
                    ?? throw new InvalidOperationException($"State for book {bookId} not found.");
        state.IsAvailable = isAvailable;
        await _context.SaveChangesAsync();
    }

    // ── Events ─────────────────────────────────────────────────────────

    public async Task<IEvent> AddEventAsync(int userId, int bookId, DateTime date, string type)
    {
        var ev = new LibraryEvent { UserId = userId, BookId = bookId, Date = date, Type = type };
        _context.Events.Add(ev);
        await _context.SaveChangesAsync();

        // Reload with navigation properties
        return await _context.Events
            .Include(e => e.User)
            .Include(e => e.Book)
            .FirstAsync(e => e.Id == ev.Id);
    }

    public async Task<IEnumerable<IEvent>> GetAllEventsAsync() =>
        await _context.Events
            .Include(e => e.User)
            .Include(e => e.Book)
            .OrderByDescending(e => e.Date)
            .ToListAsync();
}
