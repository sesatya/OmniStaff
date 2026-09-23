OmniStaff — Backend
OmniStaff is a small .NET 8 backend for leave management (users, employees, leave requests, approvals). This repository contains the API, application logic, infrastructure (EF Core + migrations) and domain models.
Supported stack
•	.NET 8
•	ASP.NET Core Web API
•	Entity Framework Core 8 (SQL Server)
•	JWT Bearer authentication
•	SMTP for email notifications
Repository layout
•	OmniStaff.Api — ASP.NET Core Web API (startup, controllers, Swagger)
•	OmniStaff.Application — application services, DTOs, interfaces
•	OmniStaff.Infrastructure — EF Core DbContext, repository implementations, migrations
•	OmniStaff.Domain — domain entities
•	OmniStaff.Api.Tests — integration/unit tests
Quickstart (local dev)
1.	Prerequisites
•	.NET 8 SDK
•	SQL Server (local or container)
•	Optional: dotnet-ef tool (see migrations)
2.	Build and run
•	From repo root: dotnet build cd OmniStaff.Api dotnet run
•	Swagger UI: https://localhost:{port}/swagger
3. JWT and Swagger
•	Use the Swagger Authorize dialog and paste: Bearer <token> (include the "Bearer " prefix).
•	Program.cs configures JwtBearer validation (Issuer, Audience and Key must match token).
4. Database migrations
•	Install EF CLI if needed: dotnet tool install --global dotnet-ef
•	Add a migration (run from repo root): dotnet ef migrations add <Name> --project OmniStaff.Infrastructure/OmniStaff.Infrastructure.csproj --startup-project OmniStaff.Api/OmniStaff.Api.csproj
•	Apply migrations: dotnet ef database update --project OmniStaff.Infrastructure/OmniStaff.Infrastructure.csproj --startup-project OmniStaff.Api/OmniStaff.Api.csproj
•	Migration files are tracked in source control. Do not add migrations to .gitignore.
