namespace Logic;

public enum LibraryChangeType { UserAdded, UserUpdated, UserDeleted, BookAdded, BookUpdated, BookDeleted, BookBorrowed, BookReturned }

public class LibraryChangedEventArgs : EventArgs
{
    public LibraryChangeType ChangeType { get; }
    public int EntityId { get; }

    public LibraryChangedEventArgs(LibraryChangeType changeType, int entityId)
    {
        ChangeType = changeType;
        EntityId = entityId;
    }
}
