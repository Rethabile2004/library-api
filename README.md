
That Swagger UI looks clean. Here's the README:

---

# LibraryAPI

A library management REST API built with ASP.NET Core Web API — featuring book and author management, JWT authentication, a borrowing system, pagination, filtering, and structured logging.

---

## Tech Stack

- **Framework:** ASP.NET Core Web API (.NET 8)
- **Database:** SQL Server (EF Core Code First)
- **Authentication:** JWT Bearer Tokens
- **Logging:** Serilog (console + file sinks)
- **ORM:** Entity Framework Core 8

---

## Features

- JWT authentication — register, login, token-based access
- Author and book management with full CRUD
- Borrowing system — borrow, return, and view borrow history
- Duplicate borrow protection — a book cannot be borrowed twice simultaneously
- User-scoped borrowing — users only see and manage their own borrow records
- DTO pattern — request and response models separated from database models
- Repository pattern — database logic decoupled from controllers
- Filtering — books by genre, title keyword, and published year
- Sorting — by title, genre, or published year
- Pagination — configurable page size across all list endpoints
- Global exception handling — consistent ProblemDetails responses
- Structured logging — every request and auth event logged with Serilog
- Data validation — enforced via Data Annotations on request DTOs

---

## Project Structure

```
LibraryAPI/
├── Controllers/          # HTTP layer — request handling and responses
├── Data/                 # AppDbContext
├── DTOs/                 # Request and response models
├── Exceptions/           # Custom exception types
├── Middleware/           # Global exception handling middleware
├── Migrations/           # EF Core database migrations
├── Models/               # Database entity models
├── Repositories/         # Data access layer
├── Services/             # Token generation service
└── Logs/                 # Runtime log files (gitignored)
```

---

## Data Model

| Entity | Description |
|---|---|
| `User` | Registered user — can borrow and return books |
| `Author` | Book author with name and biography |
| `Book` | Library book belonging to an author |
| `BorrowRecord` | Tracks borrow and return dates per user per book |

---

## API Endpoints

### Auth
| Method | Endpoint | Access | Description |
|---|---|---|---|
| POST | `/api/auth/register` | Public | Create a new account |
| POST | `/api/auth/login` | Public | Login and receive a JWT |

### Authors
| Method | Endpoint | Access | Description |
|---|---|---|---|
| GET | `/api/author` | Public | Get all authors (paginated) |
| GET | `/api/author/{id}` | Public | Get a single author |
| POST | `/api/author` | Protected | Create an author |
| DELETE | `/api/author/{id}` | Protected | Delete an author |

### Books
| Method | Endpoint | Access | Description |
|---|---|---|---|
| GET | `/api/books` | Public | Get all books (paginated, filterable) |
| GET | `/api/books/{id}` | Public | Get a single book |
| POST | `/api/books` | Protected | Create a book |
| PUT | `/api/books/{id}` | Protected | Update a book |
| DELETE | `/api/books/{id}` | Protected | Delete a book |

### Borrowing
| Method | Endpoint | Access | Description |
|---|---|---|---|
| GET | `/api/borrow/my-books` | Protected | View your borrow history |
| GET | `/api/borrow/{id}` | Protected | Get a single borrow record |
| POST | `/api/borrow/{bookId}` | Protected | Borrow a book |
| PATCH | `/api/borrow/{bookId}/return` | Protected | Return a book |

---

## Query Parameters

```
GET /api/books?searchTitle=war
GET /api/books?genre=Historical Fiction
GET /api/books?publishedYear=1949
GET /api/books?sortBy=title
GET /api/books?page=1&pageSize=5
GET /api/books?genre=Literary Fiction&sortBy=title&page=1&pageSize=5
```

---

## Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server or SQL Server LocalDB
- Thunder Client / Postman

### Setup

**1. Clone the repository:**
```bash
git clone https://github.com/Rethabile2004/library-api.git
cd library-api
```

**2. Update the connection string in `appsettings.json`:**
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=LibraryApiDb;Trusted_Connection=True;"
}
```

**3. Apply migrations:**
```bash
dotnet ef database update
```

**4. Run the project:**
```bash
dotnet run
```

**5. Open Swagger UI:**
```
https://localhost:{port}/swagger
```

**6. Seed the database:**
```
GET /api/seed/authors
GET /api/seed/books
```

---

## Authentication

Protected endpoints require a valid JWT. To authenticate:

1. Register via `POST /api/auth/register`
2. Copy the token from the response
3. Add it to your requests as a Bearer token:

```
Authorization: Bearer <your-token>
```

---

## Borrowing Flow

```
1. POST /api/borrow/{bookId}          → borrow a book
2. GET  /api/borrow/my-books          → view your history
3. PATCH /api/borrow/{bookId}/return  → return the book
4. POST /api/borrow/{bookId}          → borrow it again
```

Attempting to borrow a book that is already borrowed returns `409 Conflict`.

---

## What I Learned Building This

This project was built as an independent exercise after completing the guided TaskFlow API, applying the same patterns from scratch in a new domain:

- Designing one-to-many and many-to-many adjacent relationships in EF Core
- Implementing resource ownership — users can only access their own borrow records
- Building a borrowing system with duplicate protection using `AnyAsync`
- Applying the Repository Pattern across multiple entities independently
- Structuring a real-world API with public reads and protected writes

---

## Author

**Rethabile Eric Siase**
Advanced Diploma in Information Technology — Central University of Technology, Free State
GitHub: [@Rethabile2004](https://github.com/Rethabile2004)
