using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data;

public class LibraryEvent : IEvent
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    public int BookId { get; set; }

    [ForeignKey(nameof(BookId))]
    public Book Book { get; set; } = null!;

    public DateTime Date { get; set; }

    [Required, MaxLength(20)]
    public string Type { get; set; } = string.Empty; // "Borrow" or "Return"

    IUser IEvent.User => User;
    IBook IEvent.Book => Book;
}
