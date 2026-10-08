using Model;

namespace ViewModel;

public class EventViewModel : ViewModelBase
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string BookTitle { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Type { get; set; } = string.Empty;

    public static EventViewModel FromModel(EventModel m) =>
        new() { Id = m.Id, UserName = m.UserName, BookTitle = m.BookTitle, Date = m.Date, Type = m.Type };
}
