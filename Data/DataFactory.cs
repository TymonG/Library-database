// DataFactory is retained for test helpers that need lightweight entity creation
// without going through the database.
namespace Data;

public static class DataFactory
{
    public static User CreateUser(string name, string email) =>
        new() { Name = name, Email = email };

    public static Book CreateBook(string title, string author, int year) =>
        new() { Title = title, Author = author, Year = year };
}
