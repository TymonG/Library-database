using Model;

namespace ViewModel;

public class BookViewModel : ViewModelBase
{
    private int _id;
    private string _title = string.Empty;
    private string _author = string.Empty;
    private int _year;
    private bool _isAvailable;

    public int Id { get => _id; set => SetField(ref _id, value); }
    public string Title { get => _title; set => SetField(ref _title, value); }
    public string Author { get => _author; set => SetField(ref _author, value); }
    public int Year { get => _year; set => SetField(ref _year, value); }
    public bool IsAvailable { get => _isAvailable; set => SetField(ref _isAvailable, value); }

    public static BookViewModel FromModel(BookModel m) =>
        new() { Id = m.Id, Title = m.Title, Author = m.Author, Year = m.Year, IsAvailable = m.IsAvailable };
}
