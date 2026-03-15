# 📚 Book Lending System

> RESTful API

![.NET 9](https://img.shields.io/badge/.NET-9-512BD4) ![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927) ![Hangfire](https://img.shields.io/badge/Hangfire-1.8-orange) ![JWT](https://img.shields.io/badge/Auth-JWT-yellow)

---

## Overview

Book Lending System is a full-stack application that allows users to register, log in, and borrow or return books from a shared catalog. The backend is built with ASP.NET Core following Clean Architecture principles, while the frontend is an Angular SPA.

A Hangfire background job runs daily to automatically detect overdue borrows and log them to the database.

---

## Tech Stack

### Backend
- **ASP.NET Core (.NET 9)** — Web API
- **Entity Framework Core 9** — ORM with SQL Server
- **ASP.NET Core Identity** — Authentication & user management
- **JWT Bearer Tokens** — Stateless auth
- **Hangfire** — Background job scheduling
- **AutoMapper 16** — Object mapping
- **Clean Architecture** — Domain / Application / Infrastructure / API
- **xUnit + FluentAssertions + EF InMemory** — Unit testing

---

## Architecture

The backend follows Clean Architecture with strict dependency rules:

```
API  →  Infrastructure  →  Application  →  Domain
```

| Layer | Responsibility |
|---|---|
| **Domain** | Entities (Book, BookBorrow, OverdueBorrow, ApplicationUser), Enums — no external dependencies, Settings — for IOptions pattern |
| **Application** | Interfaces, DTOs, AutoMapper profiles, business contracts |
| **Infrastructure** | EF Core DbContext, Identity, Services, Hangfire jobs |
| **API** | Controllers, Program.cs, DI registration |

---

## Backend — Getting Started

### Prerequisites
- .NET 9 SDK
- SQL Server Express (local or remote)
- Visual Studio 2022 or VS Code

### 1. Clone the repository

```bash
git clone https://github.com/abdofathy883/BookLending-Server.git
cd book-lending-system
```

### 2. Configure appsettings.json

Update connection strings in `Api/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=BookLending;Trusted_Connection=True;",
  "HangfireConnection": "Server=.;Database=BookLending;Trusted_Connection=True;"
}
```

### 3. Apply database migrations

```bash
dotnet ef database update --project Infrastructure --startup-project Api
```

### 4. Run the API

```bash
cd Api
dotnet run
```

- API: `https://localhost:5001`
- Hangfire Dashboard: `https://localhost:5001/hangfire`

> Database migrations and admin seeding run automatically on startup.

---

## Background Jobs

Hangfire runs a daily job that checks for overdue borrows.

### OverdueBooksJob
- Runs **daily at midnight UTC**
- Queries all `BookBorrow` records where `ReturnDate < today` and `IsReturned = false`
- Inserts new entries into the `OverdueBorrows` table — skips duplicates
- Notification and logging hooks are in place for future extension

```csharp
RecurringJob.AddOrUpdate<OverdueBooksJob>(
    "check-overdue-books",
    job => job.CheckOverdueBooksAsync(),
    Cron.Daily,
    new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc }
);
```

---

## Unit Tests

Tests use **xUnit**, **FluentAssertions**, and **EF Core InMemory** — no external database required.

### Running Tests

```bash
dotnet test
```

Or use **Test Explorer** in Visual Studio 2022 (`View → Test Explorer`).

### Coverage — BookBorrowService

| Test | Scenario |
|---|---|
| ✅ | Returns DTO when book is free |
| ✅ | Throws `KeyNotFoundException` when book not found |
| ✅ | Throws `InvalidOperationException` when book is already borrowed |
| ✅ | Throws `InvalidOperationException` when book is lost |
| ✅ | Returns DTO when book is currently borrowed (return flow) |
| ✅ | Throws `KeyNotFoundException` on return when book not found |
| ✅ | Throws `InvalidOperationException` on return when book is not borrowed |
| ✅ | Inserts overdue records and returns mapped DTOs |
| ✅ | Skips duplicates already present in OverdueBorrows |
| ✅ | Returns empty list when no overdue borrows exist |

---

