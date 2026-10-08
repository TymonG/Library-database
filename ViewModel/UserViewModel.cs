using Model;

namespace ViewModel;

public class UserViewModel : ViewModelBase
{
    private int _id;
    private string _name = string.Empty;
    private string _email = string.Empty;

    public int Id { get => _id; set => SetField(ref _id, value); }
    public string Name { get => _name; set => SetField(ref _name, value); }
    public string Email { get => _email; set => SetField(ref _email, value); }

    public static UserViewModel FromModel(UserModel m) =>
        new() { Id = m.Id, Name = m.Name, Email = m.Email };
}
