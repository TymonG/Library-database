namespace Data
{
    public interface IEvent
    {
        int Id { get; }
        IUser User { get; }
        IBook Book { get; }
        DateTime Date { get; }
        string Type { get; } // "Borrow" or "Return"
    }
}