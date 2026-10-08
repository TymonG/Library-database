using Data;

namespace Model;

public class EventModel
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string BookTitle { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Type { get; set; } = string.Empty;

    public static EventModel FromEvent(IEvent ev) =>
        new() { Id = ev.Id, UserName = ev.User.Name, BookTitle = ev.Book.Title, Date = ev.Date, Type = ev.Type };
}
