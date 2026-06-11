# 📚 Library API

A RESTful library management API built with **ASP.NET Core 8**, **PostgreSQL**, and **JWT authentication**. Supports full book and author management, a borrow/return system, API versioning, rate limiting, and structured logging — deployed live on Render.

**Live:** [https://library-api-1-uxha.onrender.com](https://library-api-1-uxha.onrender.com/index.html)

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 8 Web API |
| Database | PostgreSQL via EF Core (Npgsql) |
| Auth | JWT Bearer tokens (BCrypt password hashing) |
| Logging | Serilog (console + rolling file) |
| Docs | Swagger / OpenAPI (Swashbuckle) |
| Versioning | Asp.Versioning (URL segment) |
| Rate Limiting | ASP.NET Core fixed-window limiters |
| Hosting | Render (Docker) |

---

## Features

- **JWT Authentication** — register and login with BCrypt-hashed passwords; protected endpoints require a Bearer token
- **Authors** — paginated listing, create, get by ID, delete
- **Books** — paginated listing with title/genre/year filters, create, update, delete
- **Borrow System** — borrow and return books with concurrency checks (prevents double-borrowing)
- **API Versioning** — URL segment versioning (`/api/v1/...`), version reported in response headers
- **Rate Limiting** — separate fixed-window limits for auth (5/min), write (30/min), and read (100/min) operations
- **Structured Logging** — Serilog with request logging and daily rolling log files
- **Health Check** — `/health` endpoint with live database connectivity check
- **Database Seeding** — seed endpoints for authors and books to bootstrap initial data

---

## API Endpoints

Base URL: `https://library-api-1-uxha.onrender.com/api/v1`

### Auth
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/Auth/register` | Public | Register a new user |
| POST | `/Auth/login` | Public | Login and receive a JWT |

### Authors
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/Author` | Public | List authors (paginated) |
| GET | `/Author/{id}` | Public | Get author by ID |
| POST | `/Author` | Required | Create an author |
| DELETE | `/Author/{id}` | Required | Delete an author |

### Books
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/Books` | Public | List books (paginated + filters) |
| GET | `/Books/{id}` | Public | Get book by ID |
| POST | `/Books` | Required | Create a book |
| PUT | `/Books/{id}` | Required | Update a book |
| DELETE | `/Books/{id}` | Required | Delete a book |

### Borrowing
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/Borrow/my-books` | Required | Get your borrow history |
| GET | `/Borrow/{id}` | Required | Get a borrow record by ID |
| POST | `/Borrow/{bookId}` | Required | Borrow a book |
| PATCH | `/Borrow/{bookId}/return` | Required | Return a borrowed book |

### Other
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/health` | Database connectivity check |

---

## Query Parameters

**Books** (`GET /api/v1/Books`)

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `searchTitle` | string | — | Filter by title (partial match) |
| `genre` | string | — | Filter by genre (case-insensitive) |
| `publishedYear` | int | — | Filter by year |
| `sortBy` | string | `id` | Sort by `id`, `title`, `publishedYear`, `genre` |
| `page` | int | `1` | Page number |
| `pageSize` | int | `10` | Results per page (max 50) |

**Authors** (`GET /api/v1/Author`)

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `searchName` | string | — | Filter by name |
| `sortBy` | string | `id` | Sort field |
| `page` | int | `1` | Page number |
| `pageSize` | int | `10` | Results per page (max 50) |

---

## Getting Started

### Prerequisites
- .NET 8 SDK
- PostgreSQL

### Local Setup

1. Clone the repo:
```bash
git clone https://github.com/Rethabile2004/LibraryAPI.git
cd LibraryAPI
```

2. Configure your environment in `appsettings.Development.json` or via environment variables:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=librarydb;Username=postgres;Password=yourpassword"
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key-min-32-chars",
    "Issuer": "LibraryApi",
    "Audience": "LibraryApiUsers",
    "ExpiryHours": 24
  },
  "AllowedOrigins": ["http://localhost:5173"]
}
```

3. Run the API:
```bash
dotnet run --project LibraryApi/LibraryApi.csproj
```

Migrations are applied automatically on startup. Swagger UI opens at `http://localhost:8080`.

### Seed Initial Data

Run these in order to populate the database:
```
GET /api/Seed/authors
GET /api/Seed/books
```

> **Note:** Seed authors before books — the book seed data references author IDs 1–10.

---

## Usage Examples

### Register
```bash
curl -X POST "https://library-api-1-uxha.onrender.com/api/v1/Auth/register" \
  -H "Content-Type: application/json" \
  -d '{ "email": "user@example.com", "fullName": "Your Name", "password": "StrongPassword123" }'
```

### Login
```bash
curl -X POST "https://library-api-1-uxha.onrender.com/api/v1/Auth/login" \
  -H "Content-Type: application/json" \
  -d '{ "email": "user@example.com", "password": "StrongPassword123" }'
```

### List Books with Filters
```bash
curl "https://library-api-1-uxha.onrender.com/api/v1/Books?genre=fiction&page=1&pageSize=5"
```

### Borrow a Book
```bash
curl -X POST "https://library-api-1-uxha.onrender.com/api/v1/Borrow/1" \
  -H "Authorization: Bearer <YOUR_TOKEN>"
```

### Return a Book
```bash
curl -X PATCH "https://library-api-1-uxha.onrender.com/api/v1/Borrow/1/return" \
  -H "Authorization: Bearer <YOUR_TOKEN>"
```

---

## Docker

```bash
docker build -t library-api .

docker run --rm \
  -e PORT=8080 \
  -e JwtSettings__SecretKey="your-secret" \
  -e ConnectionStrings__DefaultConnection="your-connection-string" \
  -e AllowedOrigins__0="https://your-frontend.com" \
  -p 8080:8080 \
  library-api
```

---

## Project Structure

```
LibraryApi/
├── Controllers/
│   ├── AuthController.cs
│   ├── AuthorController.cs
│   ├── BooksController.cs
│   ├── BorrowController.cs
│   └── SeedController.cs
├── Data/
│   └── AppDbContext.cs
├── DTO/
│   └── (request/response DTOs, query parameters, PagedResult)
├── Middleware/
│   └── ExceptionMiddleware.cs
├── Repositories/
│   └── (BookRepository, AuthorRepository, BorrowBookRepository + interfaces)
├── Services/
│   └── TokenService.cs
├── Migrations/
├── Logs/
└── Program.cs
```

---

## Error Handling

All errors return `application/problem+json`:

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404,
  "detail": "Book with id 99 was not found.",
  "traceId": "00-abc123..."
}
```

| Status | Meaning |
|--------|---------|
| 400 | Bad request / validation error |
| 401 | Missing or invalid JWT |
| 403 | Authenticated but not authorized |
| 404 | Resource not found |
| 409 | Conflict (e.g. book already borrowed, duplicate email) |
| 429 | Rate limit exceeded |
| 500 | Internal server error |

---

## Author

**Rethabile Eric Siase**
GitHub: [@Rethabile2004](https://github.com/Rethabile2004)

---

## License

MIT
