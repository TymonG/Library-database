// Tests for EfDataRepository using EF Core's in-memory provider.
// Production uses SQL Server (see docker-compose.yml at repo root).
// The in-memory provider is used here for test isolation only — it does not
// enforce FK constraints or SQL Server-specific behaviour.
using Data;
using Microsoft.EntityFrameworkCore;

namespace Tests.Data;

public class EfDataRepositoryTests
{
    private static LibraryDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new LibraryDbContext(options);
    }

    [Fact]
    public async Task AddUserAsync_PersistsUser()
    {
        await using var ctx = CreateInMemoryContext();
        var repo = new EfDataRepository(ctx);

        var user = await repo.AddUserAsync("Alice", "alice@test.com");

        Assert.NotEqual(0, user.Id);
        Assert.Equal("Alice", user.Name);
    }

    [Fact]
    public async Task GetAllUsersAsync_ReturnsAllUsers()
    {
        await using var ctx = CreateInMemoryContext();
        var repo = new EfDataRepository(ctx);

        await repo.AddUserAsync("Alice", "alice@test.com");
        await repo.AddUserAsync("Bob", "bob@test.com");

        var users = (await repo.GetAllUsersAsync()).ToList();
        Assert.Equal(2, users.Count);
    }

    [Fact]
    public async Task UpdateUserAsync_ChangesName()
    {
        await using var ctx = CreateInMemoryContext();
        var repo = new EfDataRepository(ctx);
        var user = await repo.AddUserAsync("Old Name", "old@test.com");

        await repo.UpdateUserAsync(user.Id, "New Name", "new@test.com");

        var updated = await repo.GetUserAsync(user.Id);
        Assert.Equal("New Name", updated!.Name);
    }

    [Fact]
    public async Task DeleteUserAsync_RemovesUser()
    {
        await using var ctx = CreateInMemoryContext();
        var repo = new EfDataRepository(ctx);
        var user = await repo.AddUserAsync("Temp", "temp@test.com");

        await repo.DeleteUserAsync(user.Id);

        Assert.Null(await repo.GetUserAsync(user.Id));
    }

    [Fact]
    public async Task AddBookAsync_CreatesBookAndState()
    {
        await using var ctx = CreateInMemoryContext();
        var repo = new EfDataRepository(ctx);

        var book = await repo.AddBookAsync("Clean Code", "Martin", 2008);

        Assert.NotEqual(0, book.Id);
        var state = await repo.GetStateAsync(book.Id);
        Assert.NotNull(state);
        Assert.True(state.IsAvailable);
    }

    [Fact]
    public async Task UpdateStateAsync_ChangesAvailability()
    {
        await using var ctx = CreateInMemoryContext();
        var repo = new EfDataRepository(ctx);
        var book = await repo.AddBookAsync("Book", "Author", 2020);

        await repo.UpdateStateAsync(book.Id, false);

        var state = await repo.GetStateAsync(book.Id);
        Assert.False(state!.IsAvailable);
    }

    [Fact]
    public async Task AddEventAsync_CreatesEvent()
    {
        await using var ctx = CreateInMemoryContext();
        var repo = new EfDataRepository(ctx);
        var user = await repo.AddUserAsync("Alice", "alice@test.com");
        var book = await repo.AddBookAsync("Book", "Author", 2020);

        var ev = await repo.AddEventAsync(user.Id, book.Id, DateTime.Now, "Borrow");

        Assert.NotEqual(0, ev.Id);
        Assert.Equal("Borrow", ev.Type);
        Assert.Equal("Alice", ev.User.Name);
    }

    [Fact]
    public async Task GetAllBooksAsync_ReturnsOrderedByTitle()
    {
        await using var ctx = CreateInMemoryContext();
        var repo = new EfDataRepository(ctx);
        await repo.AddBookAsync("Zebra", "A", 2000);
        await repo.AddBookAsync("Apple", "B", 2001);

        var books = (await repo.GetAllBooksAsync()).ToList();

        Assert.Equal("Apple", books[0].Title);
        Assert.Equal("Zebra", books[1].Title);
    }
}
