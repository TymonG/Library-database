using Data;

namespace Model;

public class UserModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public static UserModel FromUser(IUser user) =>
        new() { Id = user.Id, Name = user.Name, Email = user.Email };
}
