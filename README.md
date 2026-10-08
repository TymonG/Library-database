


### Prerequisites

- .NET 10 SDK
- Docker Desktop

### 1. Start the database

```bash
docker-compose up -d
```

This starts SQL Server 2022 on `localhost:1433` with:
- User: `sa`
- Password: `LibraryApp_2024!`

### 2. Apply migrations

Migrations are applied automatically on app startup via `MigrateAsync()`.

To apply manually (from repo root):

```bash
dotnet ef database update --project Data --startup-project View
```

To add a new migration:

```bash
dotnet ef migrations add <MigrationName> --project Data --startup-project View
```

### 3. Run the app

```bash
dotnet run --project View
```

### 4. Run tests

```bash
dotnet test
```

All 25 tests pass across three layers: `Tests/Data`, `Tests/Logic`, `Tests/ViewModel`.

---

## Project structure

```
LibraryApp.slnx
├── Data/          — EF Core entities, LibraryDbContext, EfDataRepository, Migrations
├── Logic/         — LibraryService (async), ILibraryService, LibraryChangedEventArgs
├── Model/         — DTOs (BookModel, UserModel, EventModel), LibraryModel wrapper
├── ViewModel/     — MainViewModel, BookViewModel, UserViewModel, RelayCommand, AsyncRelayCommand
├── View/          — WPF App.xaml, MainWindow.xaml (Master-Detail UI), converters
└── Tests/
    ├── Data/      — EfDataRepositoryTests (EF in-memory provider)
    ├── Logic/     — LibraryServiceTests + FakeDataRepository
    └── ViewModel/ — MainViewModelTests (Moq)
```

## Architecture

Dependency direction: `View → ViewModel → Model → Logic → Data`

- Only `View` references WPF assemblies (`UseWPF=true`, `net10.0-windows`)
- `ViewModel` uses only `System.Windows.Input` and `System.ComponentModel` (framework-agnostic)
- Both LINQ query syntax and method syntax are present in `EfDataRepository`
- All DB operations are `async` (`Task`/`Task<T>`)
- Commands are standalone classes (`RelayCommand`, `AsyncRelayCommand`) implementing `ICommand`
- Change notifications flow via `LibraryChanged` event from Logic → Model → ViewModel