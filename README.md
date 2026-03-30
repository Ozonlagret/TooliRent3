# TooliRent API

A REST API for tool rental in a makerspace environment, built with `.NET 8`, `ASP.NET Core`, `Entity Framework Core`, `SQL Server`, `JWT`, and `Swagger`.

## Features

- Register, log in, log out, and refresh tokens
- List, filter, and view tool details
- Create, view, and cancel bookings
- Mark pickup and return
- Admin features for tools, categories, user activation, and statistics

## Architecture (N-tier)

The solution is split into four layers:

- `Presentation` – API controllers, authentication configuration, Swagger
- `Application` – services, DTOs, validators, mapping
- `Domain` – entities, enums, and domain models
- `Infrastructure` – `DbContext`, repositories, `UnitOfWork`, migrations, seed data

Patterns used:

- Repository pattern for data access
- Service pattern for business logic
- DTO + mapping (custom mapper classes)

## Tech Stack

- `.NET 8`
- `ASP.NET Core Web API`
- `Entity Framework Core` (code-first)
- `SQL Server`
- `ASP.NET Identity`
- `JWT Bearer Authentication`
- `Swagger / OpenAPI`
- `FluentValidation` (validators in `Application/Validators`)

## Getting Started

### 1) Prerequisites

- Visual Studio 2022 with the **ASP.NET and web development** workload
- `.NET SDK 8`
- SQL Server (LocalDB, SQL Server Express, or full SQL Server)

### 2) Configuration

Open `Presentation/appsettings.json` and set:

- `ConnectionStrings:DefaultConnection`
- `Jwt:Key`
- `Jwt:Issuer`
- `Jwt:Audience`

### 3) Run Locally in Visual Studio (Buttons)

1. In **Solution Explorer**, right-click `Presentation` and select **Set as Startup Project**.
2. In the top toolbar, choose a launch profile (`https`, `http`, or `IIS Express`).
3. Click the green **Start** button (or press `F5`).
4. Swagger opens automatically at `/swagger`.

> Migrations and seed data run automatically at startup through `SeedData.Initialize(...)`.

### 4) Command-line Alternative

From the solution root:

```bash
dotnet restore
dotnet build
dotnet run --project Presentation
```

## Seed Data (Development)

Roles:

- `Admin`
- `Member`

Pre-seeded users:

- Admin: `admin` / `Admin123!`
- Member: `john_doe` / `Member123!`
- Member: `jane_smith` / `Member123!`
- Member: `bob_builder` / `Member123!`
- Inactive test user: `inactive_user` / `Inactive123!`

## Authentication & Authorization

- JWT is used for protected endpoints.
- Public endpoints use `[AllowAnonymous]`.
- Admin endpoints are protected with `[Authorize(Roles = "Admin")]`.

## Typical API Flow

1. `POST /public/auth/register` (optional, or use a seeded account)
2. `POST /public/auth/login` → get `token` + `refreshToken`
3. Set `Authorization: Bearer <token>` in Swagger
4. Call protected endpoints, for example `POST /member/bookings`
5. When needed, refresh access token via `POST /public/auth/refresh-token`

## Endpoint Overview

### Public Auth Endpoints

- `POST /public/auth/register`
- `POST /public/auth/login`
- `POST /public/auth/refresh-token`

### Member Auth Endpoint (requires JWT)

- `POST /member/auth/logout`

### Admin User Management Endpoints

- `GET /admin/users`
- `PATCH /admin/users/by-username/{userName}/deactivate`
- `PATCH /admin/users/by-username/{userName}/activate`
- `DELETE /admin/users/by-username/{userName}`

### Public Tool Endpoints

- `GET /public/tools/available?start={startDateTime}&end={endDateTime}`
- `GET /public/tools/filter` (query params from `ToolFilterRequest`)
- `GET /public/tools/details/{toolId}`

### Admin Tool Endpoints

- `GET /admin/tools`
- `POST /admin/tools`
- `PUT /admin/tools/{id}`
- `DELETE /admin/tools/{id}`
- `GET /admin/tools/statistics/general`
- `GET /admin/tools/statistics/usage`

### Member Booking Endpoints (requires JWT)

- `GET /member/bookings`
- `GET /member/bookings/{bookingId}`
- `POST /member/bookings`
- `DELETE /member/bookings/{bookingId}/cancel`
- `POST /member/bookings/{bookingId}/pickup`
- `POST /member/bookings/{bookingId}/return`

### Admin Booking Maintenance Endpoint

- `DELETE /admin/bookings/breakGlassInCaseOfEmergency/delete all bookings`

### Admin Tool Category Endpoints

- `GET /admin/tool-categories`
- `GET /admin/tool-categories/{id}`
- `POST /admin/tool-categories`
- `PUT /admin/tool-categories/{id}`
- `DELETE /admin/tool-categories/{id}`

## Project Structure

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

## Notes

- Validators are located in `Application/Validators`.
- Swagger and JWT are configured in `Presentation/Program.cs`.
