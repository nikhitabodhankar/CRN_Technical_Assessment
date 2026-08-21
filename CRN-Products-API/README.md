# CRN Products API

A RESTful backend API for managing **Products** (with related **Items**), built with .NET 8 / ASP.NET Core Web API following clean architecture (Domain → Application → Infrastructure → API).

## Tech Stack
- ASP.NET Core 8 Web API
- Entity Framework Core 8 + SQL Server
- JWT Bearer authentication
- FluentValidation
- AutoMapper
- Serilog (structured logging)
- Swagger / OpenAPI
- xUnit + Moq + FluentAssertions + WebApplicationFactory
- Docker & Docker Compose

## Architecture

```
Domain          -> Entities, exceptions (no dependencies)
Application     -> DTOs, interfaces, services, validators, mapping (depends on Domain)
Infrastructure  -> EF Core DbContext, repositories, Unit of Work, JWT token service (depends on Application)
API             -> Controllers, middleware, Program.cs / DI wiring (depends on Application + Infrastructure)
```

Repository + Unit of Work pattern decouples the Application layer from EF Core. Controllers are thin — all business logic sits in `ProductService`.

## Running Locally (Docker — recommended)

```bash
docker compose up --build
```

- API: http://localhost:8080/swagger
- SQL Server: localhost,1433 (sa / Your_password123)

## Running Locally (without Docker)

1. Install the [.NET 8 SDK](https://dotnet.microsoft.com/download) and a local SQL Server instance (or use `docker run` for just the DB).
2. Update the connection string in `src/API/appsettings.Development.json` if needed.
3. Create the initial migration and apply it (from the repo root):

```bash
dotnet tool install --global dotnet-ef   # if not already installed
dotnet ef migrations add InitialCreate --project src/Infrastructure --startup-project src/API
dotnet ef database update --project src/Infrastructure --startup-project src/API
```

4. Run the API:

```bash
dotnet run --project src/API
```

5. Open `https://localhost:5001/swagger` (or the URL shown in the console).

> The app also calls `Database.Migrate()` on startup, so once a migration exists, `dotnet run` alone will create/update the schema.

## Running Tests

```bash
dotnet test
```

Includes unit tests for `ProductService` (mocked repository) and integration tests for `ProductsController` using `WebApplicationFactory` with an EF Core InMemory provider.

## Authentication

This assessment doesn't require a full user-management system, so `POST /api/v1/auth/login` issues a signed JWT for any non-empty username/password — enough to exercise the `[Authorize]`-protected write endpoints (`POST`, `PUT`, `DELETE`). Reads (`GET`) are public.

```bash
curl -X POST http://localhost:8080/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"demo","password":"demo123"}'
```

Use the returned `accessToken` as `Authorization: Bearer <token>` on write requests.

## API Endpoints

| Method | Route                     | Auth | Description                          |
|--------|---------------------------|------|---------------------------------------|
| GET    | /api/v1/products          | No   | Paged list (`pageNumber`, `pageSize`, `search`) |
| GET    | /api/v1/products/{id}     | No   | Get product by id                     |
| POST   | /api/v1/products          | Yes  | Create product                        |
| PUT    | /api/v1/products/{id}     | Yes  | Update product                        |
| DELETE | /api/v1/products/{id}     | Yes  | Delete product                        |
| POST   | /api/v1/auth/login        | No   | Get a demo JWT                        |
| GET    | /health                   | No   | Health check                          |

Errors follow a consistent JSON shape via global exception middleware:

```json
{ "statusCode": 404, "message": "Product with id '5' was not found." }
```

## Notes / Trade-offs

- `dotnet ef` migrations aren't checked in (kept out of source control per common practice); generate them locally with the commands above before first run.
- Rate limiting and refresh-token rotation are called out in the assessment brief but scoped out here to keep the sample focused — `TokenService`/`AuthController` are the natural extension points.
- Pagination, `AsNoTracking()` on reads, and response compression are already wired in for the performance requirements.
