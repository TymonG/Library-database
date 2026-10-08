using Data;

namespace Model;

public class BookModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public int Year { get; set; }
    public bool IsAvailable { get; set; }

    public static BookModel FromBook(IBook book, bool isAvailable = true) =>
        new() { Id = book.Id, Title = book.Title, Author = book.Author, Year = book.Year, IsAvailable = isAvailable };
}
