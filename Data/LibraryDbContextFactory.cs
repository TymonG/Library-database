using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Data;

// Used by dotnet-ef CLI tools at design time (migrations)
public class LibraryDbContextFactory : IDesignTimeDbContextFactory<LibraryDbContext>
{
    public LibraryDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LibraryDbContext>();
        optionsBuilder.UseSqlServer(
            "Server=localhost,1433;Database=LibraryDb;User Id=sa;Password=LibraryApp_2024!;TrustServerCertificate=True");
        return new LibraryDbContext(optionsBuilder.Options);
    }
}
