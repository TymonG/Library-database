namespace Data
{
    public interface IBook
    {
        int Id { get; }
        string Title { get; }
        string Author { get; }
        int Year { get; }
    }
}