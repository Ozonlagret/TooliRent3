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

### Auth

- `POST /Auth/register`
- `POST /Auth/login`
- `POST /Auth/logout`
- `POST /Auth/refresh-token`
- `PATCH /Auth/admin/deactivate/{userId}` (Admin)
- `PATCH /Auth/admin/activate/{userId}` (Admin)

### Tools

- `GET /Tools/available` (publik)
- `GET /Tools/filter` (publik)
- `GET /Tools/details/{toolId}` (publik)
- `GET /Tools/admin/all` (Admin)
- `POST /Tools/admin` (Admin)
- `PUT /Tools/admin/{id}` (Admin)
- `DELETE /Tools/admin/{id}` (Admin)
- `GET /Tools/general-statistics` (Admin)
- `GET /Tools/usage-statistics` (Admin)

### Booking

- `GET /Booking/get-bookings`
- `GET /Booking/{bookingId}`
- `POST /Booking/create`
- `DELETE /Booking/{bookingId}/cancel`
- `POST /Booking/{bookingId}/pickup`
- `POST /Booking/{bookingId}/return`

### ToolCategory (Admin)

- `GET /admin/ToolCategory`
- `GET /admin/ToolCategory/{id}`
- `POST /admin/ToolCategory`
- `PUT /admin/ToolCategory/{id}`
- `DELETE /admin/ToolCategory/{id}`

## Projektstruktur

```text
TooliRent3/
├─ Presentation/
├─ Application/
├─ Domain/
└─ Infrastructure/
```

## Status / notering

- Validators finns i `Application/Validators`.
- Swagger och JWT är konfigurerat i `Presentation/Program.cs`.
