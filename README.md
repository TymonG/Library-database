## O programie

Library System App to desktopowa aplikacja w architekturze MVVM (WPF, .NET 8) przeznaczona do zarządzania zasobami biblioteki. Umożliwia obsługę bazy danych książek, czytelników oraz rejestrowanie wypożyczeń i zwrotów.

## Główny cel i funkcje

Program służy do automatyzacji pracy biblioteki[cite: 5]. Jego główne funkcje to:
* **Zarządzanie katalogiem:** dodawanie, edycja, usuwanie i sprawdzanie dostępności książek[cite: 5].
* **Obsługa czytelników:** rejestracja i ewidencja użytkowników[cite: 5].
* **Rejestracja zdarzeń:** zapisywanie operacji wypożyczeń i zwrotów w bazie danych[cite: 5].
* **Przechowywanie danych:** obsługa SQL Server poprzez Entity Framework Core oraz możliwość uruchomienia bazy w Dockerze[cite: 5].
-----
ENG
## About

Library System App is a desktop application built with WPF (.NET 8) using the MVVM architecture, designed for library resource management. It enables database management for books and readers, as well as tracking loans and returns.

## Purpose and Key Features

The application automates core library management operations[cite: 5]. Key features include:
* **Catalog Management:** Add, edit, remove, and check the availability of books[cite: 5].
* **User Management:** Register and track library readers[cite: 5].
* **Event Logging:** Record loan and return transactions in the database[cite: 5].
* **Data Persistence:** SQL Server support via Entity Framework Core, with optional Docker deployment[cite: 5].

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
