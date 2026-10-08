using Data;
using Logic;
using Tests.Logic;

namespace Tests.Logic;

public class LibraryServiceTests
{
    private static (LibraryService service, FakeDataRepository repo) CreateService()
    {
        var repo = new FakeDataRepository();
        return (new LibraryService(repo), repo);
    }

    [Fact]
    public async Task BorrowBook_AvailableBook_ReturnsTrue()
    {
        var (service, _) = CreateService();
        var user = await service.RegisterUserAsync("Alice", "alice@lib.com");
        var book = await service.AddBookToCatalogAsync("Clean Code", "Martin", 2008);

        bool result = await service.BorrowBookAsync(user.Id, book.Id);

        Assert.True(result);
    }

    [Fact]
    public async Task BorrowBook_MakesBookUnavailable()
    {
        var (service, _) = CreateService();
        var user = await service.RegisterUserAsync("Alice", "alice@lib.com");
        var book = await service.AddBookToCatalogAsync("Clean Code", "Martin", 2008);

        await service.BorrowBookAsync(user.Id, book.Id);

        Assert.False(await service.IsBookAvailableAsync(book.Id));
    }

    [Fact]
    public async Task BorrowBook_AlreadyBorrowed_ReturnsFalse()
    {
        var (service, _) = CreateService();
        var alice = await service.RegisterUserAsync("Alice", "alice@lib.com");
        var bob = await service.RegisterUserAsync("Bob", "bob@lib.com");
        var book = await service.AddBookToCatalogAsync("Clean Code", "Martin", 2008);

        await service.BorrowBookAsync(alice.Id, book.Id);
        bool result = await service.BorrowBookAsync(bob.Id, book.Id);

        Assert.False(result);
    }

    [Fact]
    public async Task ReturnBook_RestoresAvailability()
    {
        var (service, _) = CreateService();
        var user = await service.RegisterUserAsync("Alice", "alice@lib.com");
        var book = await service.AddBookToCatalogAsync("Clean Code", "Martin", 2008);

        await service.BorrowBookAsync(user.Id, book.Id);
        await service.ReturnBookAsync(user.Id, book.Id);

        Assert.True(await service.IsBookAvailableAsync(book.Id));
    }

    [Fact]
    public async Task BorrowAndReturn_CreatesEvents()
    {
        var (service, _) = CreateService();
        var user = await service.RegisterUserAsync("Alice", "alice@lib.com");
        var book = await service.AddBookToCatalogAsync("Clean Code", "Martin", 2008);

        await service.BorrowBookAsync(user.Id, book.Id);
        await service.ReturnBookAsync(user.Id, book.Id);

        var events = (await service.GetHistoryAsync()).ToList();
        Assert.Equal(2, events.Count);
        Assert.Equal("Borrow", events[0].Type);
        Assert.Equal("Return", events[1].Type);
    }

    [Fact]
    public async Task AllBooksAvailableInitially()
    {
        var (service, _) = CreateService();
        for (int i = 1; i <= 5; i++)
            await service.AddBookToCatalogAsync($"Book {i}", $"Author {i}", 2000 + i);

        foreach (var book in await service.GetAllBooksAsync())
            Assert.True(await service.IsBookAvailableAsync(book.Id));
    }

    [Fact]
    public async Task BorrowMultipleBooks()
    {
        var (service, _) = CreateService();
        var u1 = await service.RegisterUserAsync("Alice", "alice@lib.com");
        var u2 = await service.RegisterUserAsync("Bob", "bob@lib.com");
        var b1 = await service.AddBookToCatalogAsync("Book 1", "Author 1", 2001);
        var b2 = await service.AddBookToCatalogAsync("Book 2", "Author 2", 2002);

        Assert.True(await service.BorrowBookAsync(u1.Id, b1.Id));
        Assert.True(await service.BorrowBookAsync(u2.Id, b2.Id));
        Assert.False(await service.IsBookAvailableAsync(b1.Id));
        Assert.False(await service.IsBookAvailableAsync(b2.Id));
    }

    [Fact]
    public async Task BorrowBook_NonExistentUser_ReturnsFalse()
    {
        var (service, _) = CreateService();
        var book = await service.AddBookToCatalogAsync("Clean Code", "Martin", 2008);

        bool result = await service.BorrowBookAsync(999, book.Id);

        Assert.False(result);
    }

    [Fact]
    public async Task LibraryChanged_Event_FiredOnBorrow()
    {
        var (service, _) = CreateService();
        var user = await service.RegisterUserAsync("Alice", "alice@lib.com");
        var book = await service.AddBookToCatalogAsync("Clean Code", "Martin", 2008);

        LibraryChangedEventArgs? receivedArgs = null;
        service.LibraryChanged += (_, args) => receivedArgs = args;

        await service.BorrowBookAsync(user.Id, book.Id);

        Assert.NotNull(receivedArgs);
        Assert.Equal(LibraryChangeType.BookBorrowed, receivedArgs.ChangeType);
    }

    [Fact]
    public async Task UpdateBook_ChangesTitle()
    {
        var (service, _) = CreateService();
        var book = await service.AddBookToCatalogAsync("Old Title", "Author", 2000);

        await service.UpdateBookAsync(book.Id, "New Title", "Author", 2000);

        var updated = await service.GetBookAsync(book.Id);
        Assert.Equal("New Title", updated!.Title);
    }

    [Fact]
    public async Task DeleteBook_RemovesFromCatalog()
    {
        var (service, _) = CreateService();
        var book = await service.AddBookToCatalogAsync("To Delete", "Author", 2000);

        await service.DeleteBookAsync(book.Id);

        var all = await service.GetAllBooksAsync();
        Assert.DoesNotContain(all, b => b.Id == book.Id);
    }
}
