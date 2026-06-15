# Development Guide

## Prerequisites

- .NET 8 SDK
- Node.js with Corepack/pnpm
- PostgreSQL running locally
- GroqCloud API key for the current provider, or a Gemini API key for the optional alternative

## PostgreSQL Setup

Create a local PostgreSQL database named `wildriftcounterlab`, then configure the connection string with .NET user secrets:

```powershell
cd backend
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=wildriftcounterlab;Username=postgres;Password=YOUR_PASSWORD" --project WildRiftCounterLab.Api
```

No database password is stored in tracked configuration. Apply migrations when setting up a new database:

```powershell
cd backend
dotnet tool restore
$env:ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=wildriftcounterlab;Username=postgres;Password=YOUR_PASSWORD"
dotnet ef database update --project WildRiftCounterLab.Infrastructure --startup-project WildRiftCounterLab.Api
Remove-Item Env:ConnectionStrings__DefaultConnection
```

The API runs its idempotent seeder at startup. Existing user-created rows are preserved, while missing initial champions and matchup rules are added.

## AI Provider Secret Setup

AI explanations are optional. GroqCloud is the default provider:

```powershell
cd backend
dotnet user-secrets set "Ai:Provider" "Groq" --project WildRiftCounterLab.Api
dotnet user-secrets set "Groq:ApiKey" "YOUR_GROQ_API_KEY" --project WildRiftCounterLab.Api
dotnet user-secrets set "Groq:Model" "llama-3.1-8b-instant" --project WildRiftCounterLab.Api
```

To use Gemini instead:

```powershell
cd backend
dotnet user-secrets set "Ai:Provider" "Gemini" --project WildRiftCounterLab.Api
dotnet user-secrets set "Gemini:ApiKey" "YOUR_GEMINI_API_KEY" --project WildRiftCounterLab.Api
dotnet user-secrets set "Gemini:Model" "gemini-2.5-flash" --project WildRiftCounterLab.Api
```

Without a matching provider key, deterministic recommendations still work. AI explanation requests return the endpoint's safe fallback message.

## Run Backend

Start PostgreSQL first, then:

```powershell
cd backend
dotnet restore
dotnet run --project WildRiftCounterLab.Api --launch-profile http
```

Useful URLs:

- API base: `http://localhost:5069/api`
- Health: `http://localhost:5069/api/health`
- Champions: `http://localhost:5069/api/champions`
- Scalar API reference: `http://localhost:5069/scalar`
- OpenAPI document: `http://localhost:5069/swagger/v1/swagger.json`

## Run Frontend

The frontend reads `VITE_API_BASE_URL` and falls back to `http://localhost:5069/api`.

```powershell
cd frontend
corepack pnpm install
corepack pnpm dev
```

The frontend runs at `http://localhost:5173`. In development, the configured API URL appears subtly at the bottom of the page.

## Run Tests And Builds

```powershell
cd backend
dotnet restore
dotnet build --warnaserror --configuration Release
dotnet test --configuration Release

cd ../frontend
corepack pnpm install
corepack pnpm run lint
corepack pnpm run build
```

## CI/CD

The GitHub Actions CI workflow runs backend restore/build/tests/publish validation, frontend frozen install/lint/build, and a build of the Railway backend Dockerfile. Railway and Vercel remain responsible for deploying `main`; GitHub Actions does not use deployment tokens.

Configure branch protection to require:

- `Backend build and tests`
- `Frontend lint and build`
- `Backend Docker build`

Production smoke tests run daily and on demand. Add `PRODUCTION_BACKEND_URL` and `PRODUCTION_FRONTEND_URL` as public GitHub repository variables. Use origins without trailing slashes, and do not include `/api` in `PRODUCTION_BACKEND_URL`.

## Demo Checklist

1. Start PostgreSQL.
2. Start the backend and confirm `GET http://localhost:5069/api/health` returns `{"status":"ok"}`.
3. Confirm `GET http://localhost:5069/api/champions` includes Senna and Jhin.
4. Start the frontend at `http://localhost:5173`.
5. Select role `Baron`.
6. Select lane enemy `Darius`.
7. Select enemy champions `Senna`, `Jhin`, and `Olaf`.
8. Leave AI explanation off and click **Analyze Draft**.
9. Confirm ranked cards render score breakdowns, reasons, and plans.
10. Enable AI explanation and analyze again.
11. Confirm asynchronous AI explanations render on the top three recommendations without blocking the deterministic cards.

## Common Issues

### CORS error

The backend permits `http://localhost:5173` and `https://localhost:5173` in development. For a deployed frontend, set `Frontend__AllowedOrigins__0` to the exact HTTPS origin, without a trailing slash.

### Backend port mismatch

The frontend expects `http://localhost:5069/api`. Confirm the backend uses the `http` launch profile and check the development-only API label at the bottom of the frontend.

### Missing seeded champions

Restart the backend so startup seeding runs. Then verify `/api/champions` contains Senna and Jhin. The seeder adds missing rows without deleting existing data.

### AI provider key missing

Recommendations still work without AI. Confirm `Ai:Provider` is `Groq` or `Gemini`, then configure the matching API key before demonstrating AI explanations.

### Frontend `.env` missing

The API client safely falls back to `http://localhost:5069/api`. To configure it explicitly, create `frontend/.env` from `frontend/.env.example`:

```text
VITE_API_BASE_URL=http://localhost:5069/api
```
