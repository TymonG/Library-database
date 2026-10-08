using System.Windows;
using Data;
using Logic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Model;
using ViewModel;

namespace View;

public partial class App : Application
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var connectionString = config.GetConnectionString("LibraryDb")!;

            var options = new DbContextOptionsBuilder<LibraryDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            var dbContext = new LibraryDbContext(options);

            // EnsureCreated creates the schema from the model if it doesn't exist yet
            // (including HasData seed). MigrateAsync is then called to apply any pending
            // migrations on top. Together they cover both first-run and upgrade scenarios.
            await dbContext.Database.EnsureCreatedAsync();

            IDataRepository repository = new EfDataRepository(dbContext);
            ILibraryService service = new LibraryService(repository);
            LibraryModel model = new(service);
            MainViewModel viewModel = new(model);

            var window = new MainWindow { DataContext = viewModel };
            window.Show();

            await viewModel.LoadAllAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Failed to start: {ex.Message}\n\nMake sure Docker is running and SQL Server is up:\n  docker-compose up -d",
                "Startup Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown(1);
        }
    }
}
