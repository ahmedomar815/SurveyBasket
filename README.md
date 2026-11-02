# SurveyBasket

SurveyBasket is an ASP.NET Core Web API for publishing polls, collecting member votes, and reporting poll results. It also includes account and access administration, email notifications, background jobs, and operational health checks.

## Features

- **Poll management:** create, edit, delete, and publish/unpublish polls with a title, summary, and start/end dates.
- **Question management:** add and edit poll questions and their answer choices, activate or deactivate questions, and retrieve question lists with filtering and pagination.
- **Voting:** authenticated members can view polls currently available to them, start a vote, and submit one vote per poll. Vote answers are stored against their poll questions.
- **Results:** poll response rows, votes per day, and votes per question are available to users with the results permission.
- **Authentication and accounts:** registration, email confirmation and resend, login, JWT access tokens, refresh token renewal/revocation, forgotten-password and reset flows, profile updates, and password changes.
- **Administration:** manage users and roles, role permissions, user status and lockout, and poll/question access through permission-based authorization.
- **Email and scheduled work:** confirmation and password-reset email are queued as Hangfire jobs. Newly published polls trigger member notifications, and a daily recurring job sends new-poll notifications.
- **Operational support:** API versioning, request validation, rate limiting, centralized problem responses, structured logging, SQL Server and mail/Hangfire health checks, and a protected Hangfire dashboard.
- **Developer API reference:** OpenAPI documents and Scalar UI are exposed in Development.

## Technology and libraries

- **Platform:** .NET 9, ASP.NET Core Web API, C#.
- **Data:** Entity Framework Core 9, SQL Server, ASP.NET Core Identity, and EF Core migrations.
- **Authentication and authorization:** JWT Bearer tokens, Identity roles, and custom permission claims/policies.
- **Validation and mapping:** FluentValidation with MVC auto-validation; Mapster.
- **Background jobs:** Hangfire with SQL Server storage and its dashboard authentication filter.
- **Email:** MailKit and MimeKit.
- **Caching:** ASP.NET Core Hybrid Cache; a distributed-cache service abstraction is also present.
- **API and operations:** Asp.Versioning, ASP.NET Core OpenAPI, Scalar, Swagger UI package, health checks, Serilog, and System.Linq.Dynamic.Core.
- **Other packages:** `AspNetCore.HealthChecks.SqlServer`, `AspNetCore.HealthChecks.Hangfire`, `AspNetCore.HealthChecks.UI.Client`, `Microsoft.AspNetCore.Authentication.JwtBearer`, `Microsoft.AspNetCore.Identity.EntityFrameworkCore`, `Microsoft.Extensions.Caching.Hybrid`, and `SharpGrip.FluentValidation.AutoValidation.Mvc`.

Package versions are declared in [SurveyBasket.csproj](SurveyBasket.csproj).

## Production

The production API is available at [https://survey-basket-egypt.runasp.net](https://survey-basket-egypt.runasp.net). The authentication endpoint is `POST /auth` (for example, `https://survey-basket-egypt.runasp.net/auth`).

The supplied admin login is:

```json
{
  "email": "admin@survey-basket.com",
  "password": "P@ssword123"
}
```

**Security:** this is a production administrator password. If this README or repository is accessible to others, rotate the password immediately and remove the credential from the README and any repository history. Do not use shared default credentials for production.

## Requirements

- .NET 9 SDK
- SQL Server accessible to the application
- SMTP account for email features

## Configuration

The application requires a default SQL Server connection string and JWT settings. Hangfire uses a separate SQL Server connection string. Email and dashboard credentials are configured through `MailSettings` and `HangfireSettings`.

Set secrets with .NET User Secrets for local development (run these commands from the project directory):

```powershell
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<application SQL Server connection string>"
dotnet user-secrets set "ConnectionStrings:HangfireConnection" "<Hangfire SQL Server connection string>"
dotnet user-secrets set "Jwt:Key" "<long random signing key>"
dotnet user-secrets set "MailSettings:Mail" "<SMTP sender address>"
dotnet user-secrets set "MailSettings:DisplayName" "Survey Basket"
dotnet user-secrets set "MailSettings:Password" "<SMTP password>"
dotnet user-secrets set "MailSettings:Host" "<SMTP host>"
dotnet user-secrets set "MailSettings:Port" "587"
dotnet user-secrets set "HangfireSettings:Username" "<dashboard username>"
dotnet user-secrets set "HangfireSettings:Password" "<dashboard password>"
```

`Jwt:Issuer`, `Jwt:Audience`, and `Jwt:ExpiryMinutes` are also required. They can be set in user secrets or configuration. `AllowedOrigins` is present in app settings; review the configured CORS policy before exposing the API to a browser client.

Do not commit credentials or production connection strings. Supply production configuration through the deployment environment's secret store or environment variables.

## Run locally

From the repository root:

```powershell
dotnet restore
dotnet ef database update
dotnet run
```

The project targets `net9.0`. The launch profile provides the local URLs. In Development, open `/scalar` for the interactive API reference or `/openapi/v1.json` for the OpenAPI document. The API health endpoint is `/health`; the Hangfire dashboard is at `/jobs` and requires its configured credentials.

If the EF command is unavailable, install the tool manifest from the repository:

```powershell
dotnet tool restore
```

## API overview

The API reads an optional `x-api-version` header and assumes version 1 when omitted. Version 1 is marked deprecated in the controllers; version 2 is also declared. Poll collection endpoints use `/api/polls`.

| Area | Main routes | Purpose |
| --- | --- | --- |
| Authentication | `/Auth`, `/Auth/register`, `/Auth/refresh-token`, `/Auth/revoked-refresh-token`, `/Auth/confirm-email`, `/Auth/resend-confirm-email`, `/Auth/forget-password`, `/Auth/reset-password` | Sign-in, registration, token lifecycle, and email/password recovery |
| Current account | `/me`, `/me/update-info`, `/me/change-password` | Read and update the signed-in member's profile and password |
| Polls | `/api/polls`, `/api/polls/{id}`, `/api/polls/current`, `/api/polls/{id}/toggle-publish` | Manage polls and list currently available polls |
| Questions | `/api/polls/{pollId}/questions` and `/api/polls/{pollId}/questions/{questionId}` | Manage questions and question status |
| Votes | `/api/polls/{pollId}/votes/start`, `/api/polls/{pollId}/votes` | Retrieve available questions and submit a vote |
| Results | `/api/polls/{pollId}/results/row-data`, `/votes-perday`, `/votes-per-Question` | Poll totals and vote breakdowns |
| Users | `/User`, `/User/{userId}`, and user update/status routes | Administer users |
| Roles | `/api/roles` and role update/status routes | Administer roles and permissions |

Routes are declared in the controllers and can be inspected in the Development OpenAPI document. Administrative routes require the corresponding permission claim; member poll and vote routes require the Member role. The permission names are defined in `Abstractions/Consts/Permissions.cs`.

## Project layout

- `Controllers/` — HTTP endpoints and API version declarations.
- `Services/` — poll, question, vote, result, authentication, user, role, notification, email, and cache services.
- `Contracts/` — request/response models and FluentValidation validators.
- `Entities/` — persistence entities for polls, questions, answers, votes, users, roles, and refresh tokens.
- `Persistence/` — EF Core DbContext and entity mappings.
- `Migrations/` — SQL Server schema migrations.
- `Authentication/` — JWT configuration/provider and permission authorization.
- `Templates/` — email confirmation, password recovery, and poll notification HTML.
- `Health/` — mail provider health check.

## Data and behavior notes

- Poll titles are unique. Answer text is unique within a question, and a user can have only one vote per poll (database unique indexes).
- Entity Framework migrations manage the schema. Deletion relationships are configured to restrict cascading deletes.
- The current-polls endpoint has two API-version mappings. Version 1 has a per-user rate limit; vote endpoints use the configured concurrency limiter.
- `appsettings.json` and `appsettings.Development.json` are configuration sources. Keep any local or deployed credentials outside source control.
