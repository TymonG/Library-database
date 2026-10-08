namespace Data
{
    public interface IState
    {
        IBook Book { get; }
        bool IsAvailable { get; set; }
    }
}