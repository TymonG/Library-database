using System.Collections.ObjectModel;
using System.Windows.Input;
using Logic;
using Model;

namespace ViewModel;

public class MainViewModel : ViewModelBase
{
    private readonly LibraryModel _model;

    // ── Collections ────────────────────────────────────────────────────
    public ObservableCollection<BookViewModel> Books { get; } = new();
    public ObservableCollection<UserViewModel> Users { get; } = new();
    public ObservableCollection<EventViewModel> Events { get; } = new();

    // ── Selected items ─────────────────────────────────────────────────
    private BookViewModel? _selectedBook;
    public BookViewModel? SelectedBook
    {
        get => _selectedBook;
        set
        {
            SetField(ref _selectedBook, value);
            if (value != null)
            {
                EditBookTitle = value.Title;
                EditBookAuthor = value.Author;
                EditBookYear = value.Year;
            }
            RaiseCommandStates();
        }
    }

    private UserViewModel? _selectedUser;
    public UserViewModel? SelectedUser
    {
        get => _selectedUser;
        set
        {
            SetField(ref _selectedUser, value);
            if (value != null)
            {
                EditUserName = value.Name;
                EditUserEmail = value.Email;
            }
            RaiseCommandStates();
        }
    }

    // ── Edit fields for Detail panel – Books ───────────────────────────
    private string _editBookTitle = string.Empty;
    public string EditBookTitle { get => _editBookTitle; set => SetField(ref _editBookTitle, value); }

    private string _editBookAuthor = string.Empty;
    public string EditBookAuthor { get => _editBookAuthor; set => SetField(ref _editBookAuthor, value); }

    private int _editBookYear = DateTime.Now.Year;
    public int EditBookYear { get => _editBookYear; set => SetField(ref _editBookYear, value); }

    // ── Edit fields for Detail panel – Users ───────────────────────────
    private string _editUserName = string.Empty;
    public string EditUserName { get => _editUserName; set => SetField(ref _editUserName, value); }

    private string _editUserEmail = string.Empty;
    public string EditUserEmail { get => _editUserEmail; set => SetField(ref _editUserEmail, value); }

    // ── Status message ─────────────────────────────────────────────────
    private string _statusMessage = "Ready.";
    public string StatusMessage { get => _statusMessage; set => SetField(ref _statusMessage, value); }

    // ── Commands ───────────────────────────────────────────────────────
    public ICommand LoadDataCommand { get; }
    public ICommand AddBookCommand { get; }
    public ICommand SaveBookCommand { get; }
    public ICommand DeleteBookCommand { get; }
    public ICommand BorrowBookCommand { get; }
    public ICommand ReturnBookCommand { get; }
    public ICommand AddUserCommand { get; }
    public ICommand SaveUserCommand { get; }
    public ICommand DeleteUserCommand { get; }

    public MainViewModel(LibraryModel model)
    {
        _model = model;
        _model.LibraryChanged += OnLibraryChanged;

        LoadDataCommand = new AsyncRelayCommand(LoadAllAsync);
        AddBookCommand = new AsyncRelayCommand(AddBookAsync);
        SaveBookCommand = new AsyncRelayCommand(SaveBookAsync, _ => SelectedBook != null);
        DeleteBookCommand = new AsyncRelayCommand(DeleteBookAsync, _ => SelectedBook != null);
        BorrowBookCommand = new AsyncRelayCommand(BorrowBookAsync, _ => SelectedBook?.IsAvailable == true && SelectedUser != null);
        ReturnBookCommand = new AsyncRelayCommand(ReturnBookAsync, _ => SelectedBook?.IsAvailable == false && SelectedUser != null);
        AddUserCommand = new AsyncRelayCommand(AddUserAsync);
        SaveUserCommand = new AsyncRelayCommand(SaveUserAsync, _ => SelectedUser != null);
        DeleteUserCommand = new AsyncRelayCommand(DeleteUserAsync, _ => SelectedUser != null);
    }

    private void OnLibraryChanged(object? sender, LibraryChangedEventArgs args)
    {
        // Reload all data on any change so the UI stays consistent.
        // This runs on background thread via event, so dispatch via Task.Run / UI sync is
        // handled at the View level via data binding which marshals automatically.
        _ = LoadAllAsync();
    }

    public async Task LoadAllAsync()
    {
        try
        {
            var books = await _model.GetBooksAsync();
            var users = await _model.GetUsersAsync();
            var events = await _model.GetEventsAsync();

            Books.Clear();
            foreach (var b in books) Books.Add(BookViewModel.FromModel(b));

            Users.Clear();
            foreach (var u in users) Users.Add(UserViewModel.FromModel(u));

            Events.Clear();
            foreach (var e in events) Events.Add(EventViewModel.FromModel(e));

            StatusMessage = $"Loaded {Books.Count} books, {Users.Count} users, {Events.Count} events.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading data: {ex.Message}";
        }
    }

    private async Task AddBookAsync(object? _)
    {
        if (string.IsNullOrWhiteSpace(EditBookTitle)) return;
        try
        {
            await _model.AddBookAsync(EditBookTitle, EditBookAuthor, EditBookYear);
            EditBookTitle = string.Empty;
            EditBookAuthor = string.Empty;
            EditBookYear = DateTime.Now.Year;
            StatusMessage = "Book added.";
        }
        catch (Exception ex) { StatusMessage = $"Error: {ex.Message}"; }
    }

    private async Task SaveBookAsync(object? _)
    {
        if (SelectedBook == null) return;
        try
        {
            await _model.UpdateBookAsync(SelectedBook.Id, EditBookTitle, EditBookAuthor, EditBookYear);
            StatusMessage = "Book saved.";
        }
        catch (Exception ex) { StatusMessage = $"Error: {ex.Message}"; }
    }

    private async Task DeleteBookAsync(object? _)
    {
        if (SelectedBook == null) return;
        try
        {
            await _model.DeleteBookAsync(SelectedBook.Id);
            SelectedBook = null;
            StatusMessage = "Book deleted.";
        }
        catch (Exception ex) { StatusMessage = $"Error: {ex.Message}"; }
    }

    private async Task BorrowBookAsync(object? _)
    {
        if (SelectedBook == null || SelectedUser == null) return;
        try
        {
            var ok = await _model.BorrowBookAsync(SelectedUser.Id, SelectedBook.Id);
            StatusMessage = ok ? $"Borrowed '{SelectedBook.Title}'." : "Could not borrow — book unavailable or user/book not found.";
        }
        catch (Exception ex) { StatusMessage = $"Error: {ex.Message}"; }
    }

    private async Task ReturnBookAsync(object? _)
    {
        if (SelectedBook == null || SelectedUser == null) return;
        try
        {
            var ok = await _model.ReturnBookAsync(SelectedUser.Id, SelectedBook.Id);
            StatusMessage = ok ? $"Returned '{SelectedBook.Title}'." : "Could not return.";
        }
        catch (Exception ex) { StatusMessage = $"Error: {ex.Message}"; }
    }

    private async Task AddUserAsync(object? _)
    {
        if (string.IsNullOrWhiteSpace(EditUserName)) return;
        try
        {
            await _model.AddUserAsync(EditUserName, EditUserEmail);
            EditUserName = string.Empty;
            EditUserEmail = string.Empty;
            StatusMessage = "User added.";
        }
        catch (Exception ex) { StatusMessage = $"Error: {ex.Message}"; }
    }

    private async Task SaveUserAsync(object? _)
    {
        if (SelectedUser == null) return;
        try
        {
            await _model.UpdateUserAsync(SelectedUser.Id, EditUserName, EditUserEmail);
            StatusMessage = "User saved.";
        }
        catch (Exception ex) { StatusMessage = $"Error: {ex.Message}"; }
    }

    private async Task DeleteUserAsync(object? _)
    {
        if (SelectedUser == null) return;
        try
        {
            await _model.DeleteUserAsync(SelectedUser.Id);
            SelectedUser = null;
            StatusMessage = "User deleted.";
        }
        catch (Exception ex) { StatusMessage = $"Error: {ex.Message}"; }
    }

    private void RaiseCommandStates()
    {
        ((AsyncRelayCommand)SaveBookCommand).RaiseCanExecuteChanged();
        ((AsyncRelayCommand)DeleteBookCommand).RaiseCanExecuteChanged();
        ((AsyncRelayCommand)BorrowBookCommand).RaiseCanExecuteChanged();
        ((AsyncRelayCommand)ReturnBookCommand).RaiseCanExecuteChanged();
        ((AsyncRelayCommand)SaveUserCommand).RaiseCanExecuteChanged();
        ((AsyncRelayCommand)DeleteUserCommand).RaiseCanExecuteChanged();
    }
}
