# Wild Rift Counter Lab

Wild Rift Counter Lab is a full-stack draft assistant that recommends champion picks from deterministic matchup rules and scoring logic. Gemini can explain completed recommendations, but it never controls scores, ranking, reasons, or plans.

## Tech Stack

- Backend: ASP.NET Core Web API, Clean Architecture, Entity Framework Core, PostgreSQL
- Frontend: React, Vite, TypeScript, Tailwind CSS, Framer Motion, lucide-react, axios
- AI: Gemini explanation provider
- Tests: Application unit tests and API integration tests

## Repository Layout

```text
Wild-rift-app/
|-- WildRiftCounterLab.Api/       # Backend solution container
|-- frontend/
|   `-- wildrift-counterlab-web/  # React application
|-- .github/
|-- .gitignore
`-- README.md
```

The backend container retains its current name to avoid broad solution-path churn while active backend changes are in progress. A later dedicated rename can move it to `backend/` and update solution paths in one isolated commit.

## Architecture

The backend follows Clean Architecture:

- `WildRiftCounterLab.Domain`: entities and constants, with no infrastructure dependencies
- `WildRiftCounterLab.Application`: DTOs, services, deterministic engines, and contracts
- `WildRiftCounterLab.Infrastructure`: EF Core, repositories, seeding, and Gemini implementation
- `WildRiftCounterLab.Api`: controllers, middleware, Swagger, dependency composition, and CORS

The frontend keeps HTTP calls in `src/api`, request state in `src/hooks`, and presentation in `src/components` and `src/pages`.

## Current Features

- Deterministic draft scoring with breakdowns, reasons, and plans
- Optional AI explanations generated after recommendations are ranked
- Champion and matchup-rule CRUD APIs
- Standardized API errors and Swagger annotations
- Draft UI with champion loading, fallback data, loading/error/empty states, and recommendation cards

## Run Backend

Requirements: .NET SDK and a PostgreSQL instance matching the backend configuration.

```powershell
cd WildRiftCounterLab.Api
dotnet restore
dotnet run --project WildRiftCounterLab.Api
```

The HTTP development profile runs at `http://localhost:5069`. Swagger is available at `http://localhost:5069/swagger`.

## Run Frontend

Requirements: Node.js and pnpm through Corepack.

```powershell
cd frontend/wildrift-counterlab-web
corepack pnpm install
corepack pnpm dev
```

The Vite app runs at `http://localhost:5173`.

## Environment

The frontend example environment file contains:

```text
VITE_API_BASE_URL=http://localhost:5069/api
```

The frontend safely falls back to that URL when the variable is missing.

## Main API Routes

- `GET /api/champions`
- `POST /api/draft/recommendations`
- `POST /api/ai/explain`
- `GET|POST|PUT|DELETE /api/matchup-rules`
- `GET|POST|PUT|DELETE /api/champions`

## Verification

```powershell
cd WildRiftCounterLab.Api
dotnet restore
dotnet build --warnaserror --configuration Release
dotnet test --configuration Release

cd ../frontend/wildrift-counterlab-web
corepack pnpm install
corepack pnpm run build
```

## Roadmap

- Expand and maintain champion and matchup-rule data
- Add frontend administration screens after the draft workflow is stable
- Add authentication before exposing administration endpoints
- Move the backend container to `backend/` in a dedicated structure-only change
