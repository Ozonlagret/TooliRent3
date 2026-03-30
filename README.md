# TooliRent API

Ett REST-API för verktygsuthyrning i en makerspace-miljö, byggt med `.NET 8`, `ASP.NET Core`, `Entity Framework Core`, `SQL Server`, `JWT` och `Swagger`.

## Funktionalitet

- Registrera, logga in, logga ut och förnya token
- Lista, filtrera och visa detaljer för verktyg
- Skapa, se och avboka bokningar
- Markera upphämtning och återlämning
- Admin-funktioner för verktyg, kategorier, användaraktivering och statistik

## Arkitektur (N-tier)

Lösningen är uppdelad i fyra lager:

- `Presentation` – API Controllers, auth-konfiguration, Swagger
- `Application` – Services, DTO:er, validators, mappning
- `Domain` – Entiteter, enums och domänmodeller
- `Infrastructure` – `DbContext`, repositories, `UnitOfWork`, migrationer, seed-data

Använder:

- Repository pattern för dataåtkomst
- Service pattern för affärslogik
- DTO + mappning (egen mapper-klass)

## Teknikstack

- `.NET 8`
- `ASP.NET Core Web API`
- `Entity Framework Core` (code-first)
- `SQL Server`
- `ASP.NET Identity`
- `JWT Bearer Authentication`
- `Swagger / OpenAPI`
- `FluentValidation` (validators finns i `Application/Validators`)

## Kom igång

### 1) Förkrav

- `.NET SDK 8`
- SQL Server (lokal eller remote)

### 2) Konfiguration

Uppdatera vid behov `Presentation/appsettings.json`:

- `ConnectionStrings:DefaultConnection`
- `Jwt:Key`
- `Jwt:Issuer`
- `Jwt:Audience`

### 3) Kör API

Från lösningens rot:

```bash
dotnet restore
dotnet build
dotnet run --project Presentation
```

Vid uppstart körs migrationer + seed-data automatiskt via `SeedData.Initialize(...)`.

### 4) Swagger

Öppna:

- `https://localhost:<port>/swagger`

Använd `Authorize`-knappen och skicka JWT som `Bearer <token>`.

## Run locally (Visual Studio 2022)

### Prerequisites

- Visual Studio 2022 with the **ASP.NET and web development** workload
- .NET 8 SDK
- SQL Server (LocalDB, SQL Server Express, or full SQL Server)

### Configure app settings

1. In **Solution Explorer**, open `Presentation/appsettings.json`.
2. Set:
   - `ConnectionStrings:DefaultConnection`
   - `Jwt:Key`
   - `Jwt:Issuer`
   - `Jwt:Audience`

### Run using Visual Studio buttons (no CLI required)

1. In **Solution Explorer**, right-click the `Presentation` project and click **Set as Startup Project**.
2. At the top toolbar, select a launch profile (`https`, `http`, or `IIS Express`).
3. Click the green **Start** button (or press `F5`).
4. Swagger opens automatically at `/swagger`.

> Migrations and seed data run automatically at startup through `SeedData.Initialize(...)`.


### Command-line alternative (kept for convenience)

Från lösningens rot:

```bash
dotnet restore
dotnet build
dotnet run --project Presentation
```

## Seed-data (utveckling)

Roller:

- `Admin`
- `Member`

Förskapade användare:

- Admin: `admin` / `Admin123!`
- Member: `john_doe` / `Member123!`
- Member: `jane_smith` / `Member123!`
- Member: `bob_builder` / `Member123!`
- Inaktiv testanvändare: `inactive_user` / `Inactive123!`

## Auth & roller

- JWT används för skyddade endpoints.
- Fallback-policy kräver inloggad användare om endpoint inte explicit är `[AllowAnonymous]`.
- Admin-endpoints skyddas med `[Authorize(Roles = "Admin")]`.

## API-flöde (typiskt)

1. `POST /Auth/register` (valfritt, eller använd seed-konto)
2. `POST /Auth/login` → hämta `token` + `refreshToken`
3. Sätt `Authorization: Bearer <token>`
4. Anropa skyddade endpoints, t.ex. `POST /Booking/create`
5. Vid token-expiry: `POST /Auth/refresh-token`

## Endpoint-översikt

### Public auth endpoints

- `POST /public/auth/register`
- `POST /public/auth/login`
- `POST /public/auth/refresh-token`

### Member auth endpoint (requires JWT)

- `POST /member/auth/logout`

### Admin user management endpoints

- `GET /admin/users`
- `PATCH /admin/users/by-username/{userName}/deactivate`
- `PATCH /admin/users/by-username/{userName}/activate`
- `DELETE /admin/users/by-username/{userName}`

### Public tool endpoints

- `GET /public/tools/available?start={startDateTime}&end={endDateTime}`
- `GET /public/tools/filter` (query params from `ToolFilterRequest`)
- `GET /public/tools/details/{toolId}`

### Admin tool endpoints

- `GET /admin/tools`
- `POST /admin/tools`
- `PUT /admin/tools/{id}`
- `DELETE /admin/tools/{id}`
- `GET /admin/tools/statistics/general`
- `GET /admin/tools/statistics/usage`

### Member booking endpoints (requires JWT)

- `GET /member/bookings`
- `GET /member/bookings/{bookingId}`
- `POST /member/bookings`
- `DELETE /member/bookings/{bookingId}/cancel`
- `POST /member/bookings/{bookingId}/pickup`
- `POST /member/bookings/{bookingId}/return`

### Admin booking maintenance endpoint

- `DELETE /admin/bookings/breakGlassInCaseOfEmergency/delete all bookings`

### Admin tool category endpoints

- `GET /admin/tool-categories`
- `GET /admin/tool-categories/{id}`
- `POST /admin/tool-categories`
- `PUT /admin/tool-categories/{id}`
- `DELETE /admin/tool-categories/{id}`

## Projektstruktur

```text
TooliRent3/
├─ Presentation/
├─ Application/
├─ Domain/
└─ Infrastructure/
```

## Local URLs (from current launch settings)

- `https://localhost:7093/swagger`
- `http://localhost:5115/swagger`
- `http://localhost:13291/swagger` (IIS Express)
- `https://localhost:44318/swagger` (IIS Express SSL)

## Status / notering

- Validators finns i `Application/Validators`.
- Swagger och JWT är konfigurerat i `Presentation/Program.cs`.
