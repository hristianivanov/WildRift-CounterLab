# Wild Rift Counter Lab

Wild Rift Counter Lab is a full-stack draft assistant that recommends champion picks from role, lane matchup, and enemy-team composition. It demonstrates deterministic recommendation design, Clean Architecture, full-stack API integration, and responsible use of generative AI.

> **Design principle:** the recommendation engine decides; AI explains.

## Problem Solved

Draft decisions involve more than a single counter matchup. A useful recommendation must also consider role fit, team composition, safety, scaling, and utility. Wild Rift Counter Lab combines those signals into transparent ranked recommendations with reasons and an actionable game plan.

## Key Features

- Deterministic multi-category recommendation scoring
- Score breakdowns for lane, team fit, role fit, safety, scaling, and utility
- Rule-based reasons and game plans
- Optional Gemini explanations generated only after ranking is complete
- Champion and matchup-rule management APIs
- Idempotent initial dataset with 62 champions
- Standardized API errors, Swagger documentation, and health endpoint
- Responsive React draft workflow with loading, fallback, error, and empty states

## Architecture

```text
React + TypeScript
        |
        | HTTP / JSON
        v
ASP.NET Core API
        |
        v
Application services + deterministic engines
        |
        v
Domain entities and rules
        ^
        |
Infrastructure: PostgreSQL / EF Core / Gemini provider
```

Dependency direction:

```text
Api -> Application <- Infrastructure
          |
          v
        Domain
```

Application contains the recommendation pipeline and contracts. Infrastructure implements persistence and AI contracts. Domain remains dependency-free. See [docs/architecture.md](docs/architecture.md) for details.

## Recommendation Pipeline

1. Validate role and enemy champion selections.
2. Load champions and matching rules from PostgreSQL.
3. Calculate deterministic score categories.
4. Build rule/tag-based reasons and plans.
5. Rank and select the top recommendations.
6. Optionally ask Gemini to explain the already-completed result.

Gemini cannot change scores, ranking, reasons, plans, or score breakdowns.

## Tech Stack

| Area | Technologies |
| --- | --- |
| Frontend | React, Vite, TypeScript, Tailwind CSS, Framer Motion, axios |
| Backend | ASP.NET Core Web API, Clean Architecture |
| Data | PostgreSQL, Entity Framework Core |
| AI | Gemini API |
| Testing | xUnit, ASP.NET Core integration testing, EF Core InMemory |

## Project Structure

```text
backend/
  WildRiftCounterLab.Api
  WildRiftCounterLab.Application
  WildRiftCounterLab.Domain
  WildRiftCounterLab.Infrastructure
  WildRiftCounterLab.Api.Tests
  WildRiftCounterLab.Application.Tests
frontend/
docs/
DEVELOPMENT.md
```

The frontend separates API clients, hooks, reusable components, draft-specific components, and pages. The backend follows strict Clean Architecture dependency boundaries.

## Screenshots

Screenshot files will be added under `docs/screenshots/`.

| Draft setup | Recommendation results |
| --- | --- |
| _Placeholder_ | _Placeholder_ |

| AI explanation | Score breakdown |
| --- | --- |
| _Placeholder_ | _Placeholder_ |

## Local Setup

Detailed PostgreSQL, Gemini user-secret, troubleshooting, and demo instructions are in [DEVELOPMENT.md](DEVELOPMENT.md).

```powershell
# Backend
cd backend
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=wildriftcounterlab;Username=postgres;Password=YOUR_PASSWORD" --project WildRiftCounterLab.Api
dotnet run --project WildRiftCounterLab.Api --launch-profile http

# Frontend
cd frontend
corepack pnpm install
corepack pnpm dev
```

## Deployment

The production target is:

- Vercel for the React frontend
- Railway for the ASP.NET Core API, with Render as fallback
- Supabase PostgreSQL

The repository includes:

- `frontend/vercel.json`
- `backend/Dockerfile`
- `backend/railway.toml`
- `render.yaml`
- Optional startup migration configuration
- Environment-driven production CORS

See [docs/deployment.md](docs/deployment.md) for the complete environment-variable list, Supabase migration procedure, Gemini verification, health checks, and platform steps.

## Main API Routes

- `GET /api/health`
- `GET /api/champions`
- `POST /api/draft/recommendations`
- `POST /api/ai/explain`
- `GET|POST|PUT|DELETE /api/champions`
- `GET|POST|PUT|DELETE /api/matchup-rules`

See [docs/api.md](docs/api.md) for request and response details.

## Verification

```powershell
# Backend
cd backend
dotnet restore
dotnet build --warnaserror --configuration Release
dotnet test

# Frontend
cd ../frontend
corepack pnpm install
corepack pnpm run lint
corepack pnpm run build
```

The repository includes Application unit tests and API integration tests covering validation, deterministic ranking, AI failure safety, CRUD behavior, seed idempotency, standard errors, and health checks.

## Roadmap

Completed work, near-term improvements, and later ideas are tracked in [docs/roadmap.md](docs/roadmap.md).
