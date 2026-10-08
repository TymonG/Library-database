using System.Collections.Generic;
using Data;
using Logic;
using Model;
using Moq;
using ViewModel;

namespace Tests.ViewModel;

public class MainViewModelTests
{
    private static Mock<ILibraryService> CreateServiceMock()
    {
        var mock = new Mock<ILibraryService>();
        mock.SetupAdd(s => s.LibraryChanged += It.IsAny<EventHandler<LibraryChangedEventArgs>>());
        mock.Setup(s => s.GetAllBooksAsync()).ReturnsAsync(Enumerable.Empty<IBook>());
        mock.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(Enumerable.Empty<IUser>());
        mock.Setup(s => s.GetHistoryAsync()).ReturnsAsync(Enumerable.Empty<IEvent>());
        return mock;
    }

    [Fact]
    public async Task LoadAllAsync_PopulatesCollections()
    {
        var mock = CreateServiceMock();
        var bookMock = new Mock<IBook>();
        bookMock.Setup(b => b.Id).Returns(1);
        bookMock.Setup(b => b.Title).Returns("Clean Code");
        bookMock.Setup(b => b.Author).Returns("Martin");
        bookMock.Setup(b => b.Year).Returns(2008);
        mock.Setup(s => s.GetAllBooksAsync()).ReturnsAsync(new[] { bookMock.Object });
        mock.Setup(s => s.IsBookAvailableAsync(1)).ReturnsAsync(true);

        var vm = new MainViewModel(new LibraryModel(mock.Object));
        await vm.LoadAllAsync();

        Assert.Single(vm.Books);
        Assert.Equal("Clean Code", vm.Books[0].Title);
    }

    [Fact]
    public async Task AddBookCommand_CallsService()
    {
        var mock = CreateServiceMock();
        var bookMock = new Mock<IBook>();
        bookMock.Setup(b => b.Id).Returns(42);
        bookMock.Setup(b => b.Title).Returns("New Book");
        bookMock.Setup(b => b.Author).Returns("Author");
        bookMock.Setup(b => b.Year).Returns(2024);
        mock.Setup(s => s.AddBookToCatalogAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(bookMock.Object);

        var vm = new MainViewModel(new LibraryModel(mock.Object));
        vm.EditBookTitle = "New Book";
        vm.EditBookAuthor = "Author";
        vm.EditBookYear = 2024;

        vm.AddBookCommand.Execute(null);
        await Task.Delay(50); // allow async execution

        mock.Verify(s => s.AddBookToCatalogAsync("New Book", "Author", 2024), Times.Once);
    }

    [Fact]
    public async Task BorrowBookCommand_CannotExecute_WhenNoUserSelected()
    {
        var mock = CreateServiceMock();
        var vm = new MainViewModel(new LibraryModel(mock.Object));
        await vm.LoadAllAsync();

        var bookVm = new BookViewModel { Id = 1, Title = "Test", Author = "A", Year = 2020, IsAvailable = true };
        vm.SelectedBook = bookVm;
        vm.SelectedUser = null;

        Assert.False(vm.BorrowBookCommand.CanExecute(null));
    }

    [Fact]
    public void SelectedBook_SetToNonNull_SetsEditFields()
    {
        var mock = CreateServiceMock();
        var vm = new MainViewModel(new LibraryModel(mock.Object));

        vm.SelectedBook = new BookViewModel { Id = 1, Title = "Clean Code", Author = "Martin", Year = 2008, IsAvailable = true };

        Assert.Equal("Clean Code", vm.EditBookTitle);
        Assert.Equal("Martin", vm.EditBookAuthor);
        Assert.Equal(2008, vm.EditBookYear);
    }

    [Fact]
    public void SelectedUser_SetToNonNull_SetsEditFields()
    {
        var mock = CreateServiceMock();
        var vm = new MainViewModel(new LibraryModel(mock.Object));

        vm.SelectedUser = new UserViewModel { Id = 1, Name = "Alice", Email = "alice@test.com" };

        Assert.Equal("Alice", vm.EditUserName);
        Assert.Equal("alice@test.com", vm.EditUserEmail);
    }

    [Fact]
    public void PropertyChanged_FiredOnSelectedBook()
    {
        var mock = CreateServiceMock();
        var vm = new MainViewModel(new LibraryModel(mock.Object));
        var changedProps = new List<string?>();
        vm.PropertyChanged += (_, e) => changedProps.Add(e.PropertyName);

        vm.SelectedBook = new BookViewModel { Id = 1, Title = "T", Author = "A", Year = 2020 };

        Assert.Contains("SelectedBook", changedProps);
    }
}
