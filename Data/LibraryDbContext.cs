using Microsoft.EntityFrameworkCore;

namespace Data;

public class LibraryDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<State> States => Set<State>();
    public DbSet<LibraryEvent> Events => Set<LibraryEvent>();

    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>()
            .HasOne(b => b.State)
            .WithOne(s => s.Book)
            .HasForeignKey<State>(s => s.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<LibraryEvent>()
            .HasOne(e => e.User)
            .WithMany(u => u.Events)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<LibraryEvent>()
            .HasOne(e => e.Book)
            .WithMany(b => b.Events)
            .HasForeignKey(e => e.BookId)
            .OnDelete(DeleteBehavior.Restrict);

        // Seed data
        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Name = "Alice Smith", Email = "alice@library.com" },
            new User { Id = 2, Name = "Bob Jones", Email = "bob@library.com" },
            new User { Id = 3, Name = "Carol White", Email = "carol@library.com" }
        );

        modelBuilder.Entity<Book>().HasData(
            new Book { Id = 1, Title = "Clean Code", Author = "Robert C. Martin", Year = 2008 },
            new Book { Id = 2, Title = "The Pragmatic Programmer", Author = "Hunt & Thomas", Year = 1999 },
            new Book { Id = 3, Title = "Design Patterns", Author = "Gang of Four", Year = 1994 },
            new Book { Id = 4, Title = "Refactoring", Author = "Martin Fowler", Year = 1999 },
            new Book { Id = 5, Title = "Domain-Driven Design", Author = "Eric Evans", Year = 2003 }
        );

        modelBuilder.Entity<State>().HasData(
            new State { Id = 1, BookId = 1, IsAvailable = true },
            new State { Id = 2, BookId = 2, IsAvailable = true },
            new State { Id = 3, BookId = 3, IsAvailable = true },
            new State { Id = 4, BookId = 4, IsAvailable = true },
            new State { Id = 5, BookId = 5, IsAvailable = true }
        );
    }
}
